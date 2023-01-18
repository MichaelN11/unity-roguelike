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
    [SerializeField]
    private float attackDistance = 1;

    private GameObject target;
    private TilemapPathing tilemapPathing;

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

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        entityController = GetComponent<EntityController>();
        mainCamera = Camera.main;
    }

    private void Start()
    {
        target = PlayerController.Instance.gameObject;
        tilemapPathing = TilemapPathing.Instance;
        if (target != null)
        {
            targetBody = target.GetComponent<Rigidbody2D>();
        }
        if (tilemapPathing != null)
        {
            pathingGrid = tilemapPathing.PathingGrid;
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
                case Behavior.Move:
                    MoveAlongPath();
                    break;
                case Behavior.Attack:
                    Attack();
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
            else if (distanceToTarget <= entityController.GetAttackRange() + attackDistance)
            {
                currentBehavior = Behavior.Attack;
            }
            else
            {
                currentBehavior = Behavior.Move;
            }
        } else
        {
            currentBehavior = Behavior.Idle;
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

        Vector2 moveDirection = nextPosition - body.position;

        SendInput(InputType.Move, moveDirection);
        SendInput(InputType.Look, moveDirection);
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
            && currentBehavior == Behavior.Move
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
        Vector2 targetDirection = targetBody.position - body.position;
        SendInput(InputType.Look, targetDirection);
        SendInput(InputType.Attack);
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
