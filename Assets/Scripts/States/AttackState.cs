using System;
public class AttackState : ActionState
{
    private float duration = 1f;

    public AttackState(EntityState entityState) : base(entityState) { }

    public override bool CanAttack => false;
    public override bool CanMove => false;
    public override bool CanLook => false;

    public override void Update(float deltaTime)
    {
        duration -= deltaTime;
        if (duration <= 0)
        {
            TransitionToState(new StandState(entityState));
        }
    }
}
