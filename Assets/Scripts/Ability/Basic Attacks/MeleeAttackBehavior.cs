using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class representing a melee attack ability's behavior.
/// </summary>
public class MeleeAttackBehavior : AbilityBehavior
{
    public override float Range => NextComboData.AttackAbilityData.Range + NextComboData.AttackAbilityData.Radius;

    protected override float CastTime => NextComboData.CastTime;

    private readonly GameObject user;
    private readonly Movement movement;
    private readonly EntityState entityState;
    private readonly EntityData entityData;
    private readonly AnimatorUpdater animatorUpdater;

    private readonly MeleeAttack meleeAttack;
    private MeleeAttackComboData NextComboData => meleeAttack.ComboDataList[nextComboStage];
    private readonly int numComboStages;
    private int nextComboStage = 0;
    private float comboTimer = 0;
    private float comboableAttackTime = 0;

    public MeleeAttackBehavior(MeleeAttack meleeAttack, AbilityManager abilityManager)
    {
        this.meleeAttack = meleeAttack;
        user = UnityUtil.GetParentIfExists(abilityManager.gameObject);
        movement = user.GetComponent<Movement>();
        entityState = user.GetComponent<EntityState>();
        entityData = user.GetComponent<EntityData>();
        animatorUpdater = user.GetComponent<AnimatorUpdater>();

        numComboStages = meleeAttack.ComboDataList.Count();
        abilityManager.OnUpdate += UpdateAbility;
    }

    public override bool IsUsable(AbilityUse abilityUse)
    {
        return entityState.CanAct()
            || (entityState.ActionState == ActionState.Ability
                && entityState.StunTimer <= comboableAttackTime);
    }

    public override void Interrupt(AbilityManager component)
    {
        ResetCombo();
        base.Interrupt(component);
    }

    protected override void StartAbility(AbilityUse abilityUse)
    {
        AttackData attackData = AttackAbilityUtil.BuildAttackData(abilityUse, NextComboData.AttackAbilityData, entityData);
        attackData.AttackEvents.OnAttackSuccessful += AttackSuccessful;

        GameObject instance = AttackAbilityUtil.InstantiateDamageObject(abilityUse,
            NextComboData.AttackAbilityData,
            NextComboData.PrefabAbilityData,
            attackData);

        instance.transform.parent = user.transform;

        UpdateMovement(abilityUse.Direction);

        if (nextComboStage + 1 < numComboStages)
        {
            comboTimer = NextComboData.ComboContinueWindow;
            comboableAttackTime = NextComboData.ComboableAttackDuration;
            ++nextComboStage;
        }
        else
        {
            ResetCombo();
        }
    }

    protected override AbilityUseEventInfo OnUse(AbilityUse abilityUse)
    {
        if (movement != null)
        {
            movement.StopMoving();
        }
        if (animatorUpdater != null)
        {
            animatorUpdater.LookDirection = abilityUse.Direction;
        }
        entityState.AbilityState(NextComboData.AttackAbilityData.AttackDuration
                + NextComboData.ComboableAttackDuration
                + CastTime);
        return new AbilityUseEventInfo()
        {
            AbilityUse = abilityUse,
            AbilityAnimation = NextComboData.AttackAnimation
        };
    }

    /// <summary>
    /// Updates the ability's state. Subscribed to the AbilityManager's UpdateEvent.
    /// </summary>
    private void UpdateAbility()
    {
        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0)
            {
                ResetCombo();
            }
        }
    }

    /// <summary>
    /// Resets the combo stage.
    /// </summary>
    private void ResetCombo()
    {
        nextComboStage = 0;
        comboTimer = 0;
        comboableAttackTime = 0;
    }

    /// <summary>
    /// Updates the movement speed, direction, and acceleration.
    /// </summary>
    /// <param name="attackDirection">The direction of the attack as a vector2</param>
    private void UpdateMovement(Vector2 attackDirection)
    {
        if (movement != null)
        {
            movement.SetMovement(attackDirection,
                NextComboData.MovementAbilityData.MoveSpeed,
                NextComboData.MovementAbilityData.MoveAcceleration);
        }
    }

    /// <summary>
    /// Called after a successful attack.
    /// </summary>
    /// <param name="attackData">The attack data from the successful attack</param>
    private void AttackSuccessful(AttackData attackData)
    {
        AudioManager.Instance.Play(attackData.AbilityData.SoundOnHit);
        entityState.Stop(attackData.AbilityData.HitStop);
    }
}
