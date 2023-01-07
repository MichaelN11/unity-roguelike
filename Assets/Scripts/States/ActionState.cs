using System;
using UnityEngine;

public abstract class ActionState
{
    public abstract bool CanAttack { get; }
    public abstract bool CanMove { get; }
    public abstract bool CanLook { get; }

    protected EntityState entityState;

    public ActionState(EntityState entityState)
    {
        this.entityState = entityState;
    }

    protected void TransitionToState(ActionState actionState)
    {
        Exit();
        //entityState.Action = actionState;
        actionState.Enter();
    }

    public virtual void Update(float deltaTime) {}
    protected virtual void Enter() {}
    protected virtual void Exit() {}
}
