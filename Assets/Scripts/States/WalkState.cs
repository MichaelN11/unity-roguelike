using System;
public class WalkState : ActionState
{
    public WalkState(EntityState entityState) : base(entityState) { }

    public override bool CanAttack => true;
    public override bool CanMove => true;
    public override bool CanLook => true;
}
