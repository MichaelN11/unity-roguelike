using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface that represents a search problem to be solved by a search algorithm
/// such as A* search.
/// </summary>
/// <typeparam name="State">The type of state being evaluated</typeparam>
/// <typeparam name="Action">The type of action being taken on the state</typeparam>
public interface ISearchProblem<State, Action>
{
    public State GetStartState();
    public float GetCost(List<Action> actions);
    public float Heuristic(State state);
    public bool IsGoalState(State state);
    public List<(State, Action)> GetSuccessors(State state);
}
