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
    /// <summary>
    /// The distance away from a position required to start moving to the next position.
    /// </summary>
    private const float MovementBuffer = 0.5f;

    [SerializeField]
    private float timeBetweenAIUpdate = 1f;
    [SerializeField]
    private float aggroDistance = 5;
    /// <summary>
    /// The distance at which we no longer need to check for line of sight to attack.
    /// </summary>
    [SerializeField]
    private float meleeRadius = 1;
    /// <summary>
    /// The y offset for aiming attacks. We want to aim slightly higher than the center of the target.
    /// </summary>
    [SerializeField]
    private float attackTargetYOffset = 0.1f;

    private GameObject target;
    private Level level;

    private PathingGrid pathingGrid;
    private Rigidbody2D body;
    private Rigidbody2D targetBody;
    private EntityController entityController;

    private List<GridAction> movementPath = new();
    private int nextPathStep = 0;
    private Vector2 nextPosition = Vector2.zero;
    private bool active = false;
    private Behavior currentBehavior;
    private Camera mainCamera;

    private Collider2D movementCollider;
    private List<RaycastHit2D> raycastHits = new();
    private ContactFilter2D contactFilter2D = new();

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        entityController = GetComponent<EntityController>();
        movementCollider = GetComponent<Collider2D>();
        mainCamera = Camera.main;
        contactFilter2D.layerMask = LayerUtil.GetUnwalkableLayerMask();
        contactFilter2D.useLayerMask = true;
    }

    private void Start()
    {
        target = PlayerController.Instance.gameObject;
        level = Level.Instance;
        if (target != null)
        {
            targetBody = target.GetComponent<Rigidbody2D>();
        }
        if (level != null)
        {
            pathingGrid = level.PathingGrid;
        }
        nextPosition = body.position;
        InvokeRepeating(nameof(FindPath), 0, timeBetweenAIUpdate);
    }

    private void Update()
    {
        DetermineIfActive();
        if (active) {
            DetermineBehavior();
            switch(currentBehavior)
            {
                case Behavior.Path:
                    MoveAlongPath();
                    break;
                case Behavior.Attack:
                    Attack();
                    break;
                case Behavior.Chase:
                    MoveTowardsPoint(targetBody.position);
                    break;
            }
        }
    }

    /// <summary>
    /// Determines if the AI entity is active by checking if it's on the current screen.
    /// </summary>
    private void DetermineIfActive()
    {
        if (IsOnScreen(body.position))
        {
            active = true;
        } else
        {
            active = false;
            SendInput(InputType.Idle);
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
    /// Determines the current AI behavior by checking the distance from the target's position.
    /// </summary>
    private void DetermineBehavior()
    {
        if (targetBody != null)
        {
            float distanceToTarget = Vector2.Distance(body.position, targetBody.position);
            if (distanceToTarget > aggroDistance)
            {
                currentBehavior = Behavior.Idle;
                SendInput(InputType.Idle);
            }
            else if (CanAttack(entityController.GetAbilitySourcePosition(), GetAttackTargetPosition(),
                distanceToTarget, entityController.GetAttackRange()))
            {
                currentBehavior = Behavior.Attack;
            }
            else
            {
                DetermineMovementBehavior(distanceToTarget);
            }
        } else
        {
            currentBehavior = Behavior.Idle;
        }
    }

    /// <returns>true if the entity can attack the target</returns>
    private bool CanAttack(Vector2 position, Vector2 targetPosition, float distanceToTarget, float attackRange)
    {
        bool canAttack = false;
        if (distanceToTarget <= attackRange)
        {
            if (distanceToTarget <= meleeRadius)
            {
                canAttack = true;
            }
            else
            {
                canAttack = HasLineOfSightOnTarget(position, targetPosition, distanceToTarget);
            }
        }
        return canAttack;
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
        return targetBody.position + new Vector2(0, attackTargetYOffset);
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
                nextPosition = pathingGrid.NodeToWorld(movementPath[nextPathStep++].Node);
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
            movementPath = AStarSearch<GridNode, GridAction>.AStar(search, 200);
            nextPathStep = 0;
        }
    }

    /// <summary>
    /// Attack in the direction of the target.
    /// </summary>
    private void Attack()
    {
        Vector2 targetDirection = GetAttackTargetPosition() - body.position;
        SendInput(InputType.Attack, targetDirection);
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

    /// <summary>
    /// Passes input to the EntityController.
    /// </summary>
    /// <param name="inputType">The InputType</param>
    private void SendInput(InputType inputType)
    {
        SendInput(inputType, Vector2.zero);
    }

    /// <summary>
    /// Passes input to the EntityController.
    /// </summary>
    /// <param name="inputType">The InputType</param>
    /// <param name="direction">The Vector2 direction</param>
    private void SendInput(InputType inputType, Vector2 direction)
    {
        InputData inputData = new();
        inputData.Type = inputType;
        inputData.Direction = direction;
        entityController.UpdateFromInput(inputData);
    }
}
