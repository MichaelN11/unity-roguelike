using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

/// <summary>
/// Class used to implement the A* search algorithm.
/// </summary>
/// <typeparam name="State">The type of state being evaluated</typeparam>
/// <typeparam name="Action">The type of action being taken on the state</typeparam>
public class AStarSearch<State, Action>
{
    /// <summary>
    /// Runs A* search on the passed search problem, either until the goal is found,
    /// there is no solution found after every state is evaluated, or the max number
    /// of iterations is hit.
    /// </summary>
    /// <param name="problem">The search problem A* is running on</param>
    /// <param name="maxIterations">The maximum number of iterations before the search is force stopped</param>
    /// <returns>The best path list of actions to reach the goal, if the max iterations weren't hit</returns>
    static public List<Action> AStar(ISearchProblem<State, Action> problem, int maxIterations = 1000)
    {
        int iterationNum = 0;

        SimplePriorityQueue<(State, List<Action>)> fringe = new();
        HashSet<State> closedSet = new();

        fringe.Enqueue((problem.GetStartState(), new List<Action>()), 0);

        while (fringe.Count > 0)
        {
            (State state, List<Action> actions) next = fringe.Dequeue();

            if (problem.IsGoalState(next.state))
            {
                return next.actions;
            }

            if (!closedSet.Contains(next.state))
            {
                closedSet.Add(next.state);
                List<(State, Action)> successors = problem.GetSuccessors(next.state);

                if (++iterationNum > maxIterations)
                {
                    Debug.Log("A* is iterating too much");
                    break;
                }

                foreach ((State state, Action action) in successors)
                {
                    List<Action> newActions = new(next.actions);
                    newActions.Add(action);
                    float priority = problem.GetCost(newActions) + problem.Heuristic(state);
                    fringe.Enqueue((state, newActions), priority);
                }
            }
        }

        return new List<Action>();
    }
}
