using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField]
    private GameObject target;
    [SerializeField]
    private TilemapPathing tilemapPathing;

    private PathingGrid pathingGrid;
    private Rigidbody2D body;
    private Rigidbody2D targetBody;
    private EntityController entityController;

    // Start is called before the first frame update
    void Start()
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
    }

    // Update is called once per frame
    void Update()
    {
        if (body != null
            && targetBody != null
            && pathingGrid != null
            && pathingGrid.Grid.Count > 0)
        {
            //Debug.Log("AI grid x: " + pathingGrid.Grid.Count + " grid y: " + pathingGrid.Grid[0].Count);
            Vector2Int thisCell = WorldToCell(body.position);
            Vector2Int targetCell = WorldToCell(targetBody.position);
            Debug.Log("grid x size: " + pathingGrid.Grid.Count + " grid y size: " +
                pathingGrid.Grid.Count + " thisCell: " + thisCell);
            GridNode thisNode = pathingGrid.Grid[thisCell.x][thisCell.y];
            GridNode targetNode = pathingGrid.Grid[targetCell.x][targetCell.y];
            GridSearchProblem search = new(thisNode, targetNode);
            List<GridAction> path = AStarSearch<GridNode, GridAction>.AStar(search);
            Vector2 pathPosition = pathingGrid.GridLayout.CellToWorld(
                new Vector3Int(path[0].Node.X, path[0].Node.Y, 0));

            InputData inputData = new();
            inputData.Type = InputType.Move;
            inputData.Direction = pathPosition - body.position;
            entityController.UpdateFromInput(inputData);

            InputData inputData2 = new();
            inputData2.Type = InputType.Look;
            inputData2.Direction = pathPosition - body.position;
            entityController.UpdateFromInput(inputData2);
        }
    }

    Vector2Int WorldToCell(Vector2 position)
    {
        int x = (int) ((position.x - pathingGrid.GridLayout.transform.position.x) / pathingGrid.GridLayout.cellSize.x);
        int y = (int)((position.y - pathingGrid.GridLayout.transform.position.y) / pathingGrid.GridLayout.cellSize.y);
        return new Vector2Int(x, y);
    }
}
