using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for controlling an entity using AI.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(EntityController))]
public class AIController : MonoBehaviour
{
    private const float TimeBetweenAIUpdate = 1f;
    /// <summary>
    /// The distance away from a position required to start moving to the next position.
    /// </summary>
    private const float MovementBuffer = 0.5f;
    /// <summary>
    /// The distance at which we no longer need to check for line of sight to attack.
    /// </summary>
    private const float MeleeRadius = 1;
    /// <summary>
    /// The y offset for aiming attacks. We want to aim slightly higher than the center of the target.
    /// </summary>
    private const float AttackTargetYOffset = 0.1f;
    /// <summary>
    /// The maximum number of iterations to perform in the A* search.
    /// </summary>
    private const int MaxIterations = 400;
    /// <summary>
    /// The time between the entity deciding to use an ability, and redetermining which ability to use.
    /// </summary>
    private const float TimeToDetermineAbility = 5;

    private EntityAI entityAI;

    private GameObject target;
    private LevelManager level;

    private PathingGrid pathingGrid;
    private Rigidbody2D body;
    private Rigidbody2D targetBody;
    private EntityController entityController;
    private AbilityManager abilityManager;
    private EntityData entityData;

    private List<GridAction> movementPath = new();
    private int nextPathStep = 0;
    private Vector2 nextPosition = Vector2.zero;
    private bool active = false;
    private Behavior currentBehavior;
    private Camera mainCamera;

    private UsableAbilityInfo currentAbility = null;
    private float abilityDeterminationTimer = 0;

    private Collider2D movementCollider;
    private List<RaycastHit2D> raycastHits = new();
    private ContactFilter2D contactFilter2D = new();

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        entityController = GetComponent<EntityController>();
        movementCollider = GetComponent<Collider2D>();
        abilityManager = GetComponentInChildren<AbilityManager>();
        entityData = GetComponent<EntityData>();
        mainCamera = Camera.main;
        contactFilter2D.layerMask = LayerUtil.GetUnwalkableLayerMask();
        contactFilter2D.useLayerMask = true;
    }

    private void Start()
    {
        target = PlayerController.Instance.gameObject;
        level = LevelManager.Instance;
        if (target != null)
        {
            targetBody = target.GetComponent<Rigidbody2D>();
        }
        if (level != null)
        {
            pathingGrid = level.PathingGrid;
        }
        nextPosition = body.position;
        InvokeRepeating(nameof(FindPath), 0, TimeBetweenAIUpdate);
        GoIdle();
    }

    private void Update()
    {
        DetermineIfActive();
        if (targetBody == null)
        {
            if (currentBehavior != Behavior.Idle)
            {
                GoIdle();
            }
        } else if (active)
        {
            DetermineBehavior();
            if (currentBehavior != Behavior.Idle)
            {
                DetermineAbility();
            }
            switch(currentBehavior)
            {
                case Behavior.Path:
                    MoveAlongPath();
                    break;
                case Behavior.Ability:
                    UseAbility();
                    break;
                case Behavior.Chase:
                    MoveTowardsPoint(targetBody.position);
                    break;
            }
        }
    }

    /// <summary>
    /// Creates a new AIController component and adds it to the passed object.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="entityAI"></param>
    /// <returns></returns>
    public static AIController AddToObject(GameObject gameObject, EntityAI entityAI)
    {
        AIController aiController = gameObject.AddComponent<AIController>();
        aiController.entityAI = entityAI;
        return aiController;
    }

    /// <summary>
    /// Determines if the AI entity is active by checking if it's on the current screen.
    /// </summary>
    private void DetermineIfActive()
    {
        if (IsOnScreen(body.position))
        {
            active = true;
        } else if (active)
        {
            active = false;
            GoIdle();
        }
    }

    /// <summary>
    /// Determines if the passed position is on the screen, using the main camera.
    /// Checks it generously so that the object will be on screen, if part of the
    /// sprite is visible.
    /// </summary>
    /// <param name="worldPosition">The world position as a Vector2</param>
    /// <returns>true if the position is on the screen</returns>
    private bool IsOnScreen(Vector2 worldPosition)
    {
        bool isOnScreen = false;
        if (mainCamera != null)
        {
            Vector3 viewportPosition = mainCamera.WorldToViewportPoint(worldPosition);
            isOnScreen = viewportPosition.x >= -0.05
                   && viewportPosition.x <= 1.05
                   && viewportPosition.y >= -0.05
                   && viewportPosition.y <= 1.05;
        }
        return isOnScreen;
    }

    /// <summary>
    /// Determines which ability the AI wants to use. The AI will decide on the ability, then try
    /// to put themselves into a situation to use it.
    /// </summary>
    private void DetermineAbility()
    {
        if (abilityDeterminationTimer > 0)
        {
            abilityDeterminationTimer -= Time.deltaTime;
        }
        if (abilityDeterminationTimer <= 0 && abilityManager != null)
        {
            List<UsableAbilityInfo> usableAbilities = abilityManager.GetUsableAbilities();
            if (currentAbility != null)
            {
                usableAbilities.RemoveAll(usableAbility => usableAbility.AbilityNumber == currentAbility.AbilityNumber);
            }
            if (usableAbilities.Count > 0)
            {
                int nextIndex = UnityEngine.Random.Range(0, usableAbilities.Count);
                currentAbility = usableAbilities[nextIndex];
            }
            abilityDeterminationTimer = TimeToDetermineAbility;
        }
    }

    /// <summary>
    /// Determines the current AI behavior by checking the distance from the target's position.
    /// </summary>
    private void DetermineBehavior()
    {
        float distanceToTarget = Vector2.Distance(body.position, targetBody.position);
        if (entityAI.Deaggro
            && currentBehavior != Behavior.Idle
            && distanceToTarget > entityAI.AggroDistance)
        {
            GoIdle();
        }
        if (currentBehavior != Behavior.Idle || distanceToTarget <= entityAI.AggroDistance)
        {
            if (CanUseCurrentAbility(GetAbilitySourcePosition(), GetAttackTargetPosition(), distanceToTarget))
            {
                currentBehavior = Behavior.Ability;
            }
            else
            {
                DetermineMovementBehavior(distanceToTarget);
            }
        }
    }

    /// <returns>true if the entity can use the current ability</returns>
    private bool CanUseCurrentAbility(Vector2 position, Vector2 targetPosition, float distanceToTarget)
    {
        if (currentAbility == null || currentAbility.Ability == null)
        {
            return false;
        }

        float range = entityData.Entity.InteractionDistance + currentAbility.Ability.Range;
        bool canUseAbility = false;
        if (distanceToTarget <= range)
        {
            if (distanceToTarget <= MeleeRadius)
            {
                canUseAbility = true;
            }
            else
            {
                canUseAbility = HasLineOfSightOnTarget(position, targetPosition, distanceToTarget);
            }
        }
        return canUseAbility;
    }

    /// <returns>true if the entity has line of sight (no walls in the way) on the target position</returns>
    private bool HasLineOfSightOnTarget(Vector2 position, Vector2 targetPosition, float distance)
    {
        bool lineOfSight = false;
        ContactFilter2D contactFilter = new()
        {
            useLayerMask = true,
            layerMask = LayerUtil.GetWallLayerMask()
        };
        int collisionCount = Physics2D.Raycast(position, targetPosition - position,
            contactFilter, raycastHits, distance);
        if (collisionCount == 0)
        {
            lineOfSight = true;
        }
        return lineOfSight;
    }

    /// <summary>
    /// Determines the attack target position. The target's position plus a y offset to aim higher than the target's center.
    /// </summary>
    /// <returns>The attack target position as a Vector2</returns>
    private Vector2 GetAttackTargetPosition()
    {
        return targetBody.position + new Vector2(0, AttackTargetYOffset);
    }

    private Vector2 GetAbilitySourcePosition()
    {
        return (abilityManager != null) ? abilityManager.transform.position : transform.position;
    }

    /// <summary>
    /// Determines the AI's movement behavior. If there is no solid object in the path of the
    /// target, the AI will chase directly towards it. Otherwise, pathfind towards the target.
    /// </summary>
    /// <param name="distanceToTarget">The distance away from the current target</param>
    private void DetermineMovementBehavior(float distanceToTarget)
    {
        int collisionCount;
        collisionCount = (movementCollider != null) ?
            movementCollider.Cast(targetBody.position - body.position,
            contactFilter2D,
            raycastHits,
            distanceToTarget)
            : 0;
        if (collisionCount == 0)
        {
            currentBehavior = Behavior.Chase;
        } else
        {
            Behavior oldBehavior = currentBehavior;
            currentBehavior = Behavior.Path;
            if (oldBehavior != Behavior.Path)
            {
                FindPath();
            }
        }
    }

    /// <summary>
    /// Moves along the movement path.
    /// </summary>
    private void MoveAlongPath()
    {
        if (nextPathStep == 0 || IsNextPositionReached())
        {
            if (nextPathStep < movementPath.Count)
            {
                nextPosition = pathingGrid.NodeToWorldWithOffset(movementPath[nextPathStep++].Node);
            } else
            {
                nextPosition = targetBody.position;
            }
        }

        MoveTowardsPoint(nextPosition);
    }

    /// <summary>
    /// Determines if the next position has been reached, by checking if the current position
    /// is within the movement buffer of the next position.
    /// </summary>
    /// <returns>true if the next position has been reached</returns>
    private bool IsNextPositionReached()
    {
        float distanceFromNextPosition = Vector2.Distance(nextPosition, body.position);
        return distanceFromNextPosition < MovementBuffer;
    }

    /// <summary>
    /// Finds a movement path to the target using A* search.
    /// </summary>
    private void FindPath()
    {
        if (active
            && currentBehavior == Behavior.Path
            && targetBody != null
            && pathingGrid != null
            && pathingGrid.Grid.Count > 0)
        {
            GridNode thisNode = pathingGrid.WorldToNode(body.position);
            GridNode targetNode = pathingGrid.WorldToNode(targetBody.position);

            GridSearchProblem search = new(thisNode, targetNode);
            movementPath = AStarSearch<GridNode, GridAction>.AStar(search, MaxIterations);
            nextPathStep = 0;
        }
    }

    /// <summary>
    /// Uses ability in the direction of the target.
    /// </summary>
    private void UseAbility()
    {
        Vector2 targetDirection = GetAttackTargetPosition() - body.position;
        SendInput(InputType.Ability, targetDirection, currentAbility.AbilityNumber);
    }

    /// <summary>
    /// Move towards the target point.
    /// </summary>
    /// <param name="point">The target to move to</param>
    private void MoveTowardsPoint(Vector2 point)
    {
        Vector2 moveDirection = point - body.position;

        SendInput(InputType.Move, moveDirection);
        SendInput(InputType.Look, moveDirection);
    }

    private void GoIdle()
    {
        bool success = SendInput(InputType.Idle);
        if (success)
        {
            currentBehavior = Behavior.Idle;
        }
    }

    /// <summary>
    /// Passes input to the EntityController.
    /// </summary>
    /// <param name="inputType">The InputType</param>
    /// <returns>if the input was successful</returns>
    private bool SendInput(InputType inputType)
    {
        return SendInput(inputType, Vector2.zero);
    }

    /// <summary>
    /// Passes input to the EntityController.
    /// </summary>
    /// <param name="inputType">The InputType</param>
    /// <param name="direction">The Vector2 direction</param>
    /// <param name="abilityNumber">The number of the ability being used</param>
    /// <returns>if the input was successful</returns>
    private bool SendInput(InputType inputType, Vector2 direction, int abilityNumber = -1)
    {
        InputData inputData = new();
        inputData.Type = inputType;
        inputData.Direction = direction;
        if (abilityNumber >= 0)
        {
            inputData.AbilityNumber = abilityNumber;
        }
        return entityController.UpdateFromInput(inputData);
    }
}
