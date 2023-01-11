using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for controlling an entity using AI.
/// </summary>
public class AIController : MonoBehaviour
{
    /// <summary>
    /// The distance away from a position required to start moving to the next position.
    /// </summary>
    private const float MovementBuffer = 0.5f;

    [SerializeField]
    private GameObject target;
    [SerializeField]
    private TilemapPathing tilemapPathing;

    private PathingGrid pathingGrid;
    private Rigidbody2D body;
    private Rigidbody2D targetBody;
    private EntityController entityController;

    private List<GridAction> movementPath = new();
    private int nextPathStep = 0;
    private Vector2 nextPosition = Vector2.zero;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        entityController = GetComponent<EntityController>();
        if (tilemapPathing != null)
        {
            pathingGrid = tilemapPathing.PathingGrid;
        }
        if (target != null)
        {
            targetBody = target.GetComponent<Rigidbody2D>();
        }

        nextPosition = body.position;
        InvokeRepeating(nameof(FindPath), 0, 1);
    }

    private void Update()
    {
        MoveAlongPath();
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
        if (body != null
            && targetBody != null
            && pathingGrid != null
            && pathingGrid.Grid.Count > 0)
        {
            GridNode thisNode = pathingGrid.WorldToNode(body.position);
            GridNode targetNode = pathingGrid.WorldToNode(targetBody.position);

            GridSearchProblem search = new(thisNode, targetNode);
            movementPath = AStarSearch<GridNode, GridAction>.AStar(search);
            nextPathStep = 0;
        }
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
