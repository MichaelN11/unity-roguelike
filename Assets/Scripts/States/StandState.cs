using System;
using UnityEngine;

public class StandState : ActionState
{
    public StandState(EntityState entityState) : base(entityState) {}

    public override bool CanAttack => true;
    public override bool CanMove => true;
    public override bool CanLook => true;
}
