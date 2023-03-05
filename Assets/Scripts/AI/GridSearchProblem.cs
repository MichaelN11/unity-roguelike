using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Search problem for finding the shortest path to a position in a grid, with
/// 8-directional movement.
/// </summary>
public class GridSearchProblem : ISearchProblem<GridNode, GridAction>
{
    private readonly GridNode startNode;
    private readonly GridNode goalNode;

    public GridSearchProblem(GridNode startNode, GridNode goalNode)
    {
        this.startNode = startNode;
        this.goalNode = goalNode;
    }

    public float GetCost(List<GridAction> actions)
    {
        float totalCost = 0;
        foreach (GridAction action in actions)
        {
            totalCost += action.Cost;
        }
        return totalCost;
    }

    public GridNode GetStartState()
    {
        return startNode;
    }

    public List<(GridNode, GridAction)> GetSuccessors(GridNode state)
    {
        List<(GridNode, GridAction)> successors = new();
        foreach (GridAction action in state.AdjacentActions) {
            if (action.Node.Passable)
            {
                successors.Add((action.Node, action));
            }
        }
        return successors;
    }

    public float Heuristic(GridNode state)
    {
        return CalculateDistanceCost(state, goalNode);
    }

    public bool IsGoalState(GridNode state)
    {
        return state == goalNode;
    }

    /// <summary>
    /// Calculates the distance cost between the passed start node and passed goal node.
    /// </summary>
    /// <param name="start">The start GridNode</param>
    /// <param name="goal">The goal GridNode</param>
    /// <returns>The distance cost between the nodes</returns>
    private float CalculateDistanceCost(GridNode start, GridNode goal)
    {
        float xDistance = Mathf.Abs(goal.X - start.X);
        float yDistance = Mathf.Abs(goal.Y - start.Y);
        float remaining = Mathf.Abs(xDistance - yDistance);
        float totalAdjacentCost = remaining * PathingGrid.StraightMoveCost;
        return totalAdjacentCost;
    }
}
