using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class representing a melee attack ability's behavior.
/// </summary>
public class MeleeAttackBehavior : AbilityBehavior
{
    private readonly MeleeAttack meleeAttack;
    public override Ability Ability => meleeAttack;

    public override float Range => NextComboData.AttackAbilityData.Range + NextComboData.AttackAbilityData.Radius;

    protected override float CastTime => NextComboData.CastTime;

    private readonly GameObject user;
    private readonly Movement movement;
    private readonly EntityState entityState;
    private readonly EntityData entityData;
    private readonly AnimatorUpdater animatorUpdater;

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
        abilityManager.UpdateEvent += UpdateAbility;
    }

    public override bool IsUsable(AbilityUse abilityUse)
    {
        return entityState.CanAct()
            || (entityState.ActionState == ActionState.UsingAbility
                && entityState.StunTimer <= comboableAttackTime);
    }

    public override void Interrupt(AbilityManager component)
    {
        ResetCombo();
        base.Interrupt(component);
    }

    protected override void StartAbility(AbilityUse abilityUse)
    {
        Vector2 distance = abilityUse.Direction.normalized * NextComboData.AttackAbilityData.Range;
        Vector3 position = abilityUse.Position + distance;
        GameObject instance = Object.Instantiate(NextComboData.PrefabAbilityData.Prefab,
            position,
            DetermineRotation(abilityUse.Direction));

        instance.transform.parent = abilityUse.Component.gameObject.transform;

        DestroyTimer destroyTimer = instance.GetComponent<DestroyTimer>();
        destroyTimer.Duration = NextComboData.PrefabAbilityData.PrefabDuration;

        DamageObject attackObject = instance.GetComponent<DamageObject>();
        attackObject.AttackData = BuildAttackData(abilityUse);

        AudioManager.Instance.Play(NextComboData.AttackAbilityData.SoundOnUse);

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

    /// <summary>
    /// Method called when the ability is successfully used. This is called before the initial cast time.
    /// </summary>
    /// <param name="abilityUse">AbilityUse object containing data about how the ability was used</param>
    protected override void OnUse(AbilityUse abilityUse)
    {
        if (movement != null)
        {
            movement.StopMoving();
        }
        if (animatorUpdater != null)
        {
            animatorUpdater.AttackAnimation = NextComboData.AttackAbilityData.AttackAnimation;
            animatorUpdater.LookDirection = abilityUse.Direction;
        }
        entityState.AbilityState(NextComboData.AttackAbilityData.AttackDuration
                + NextComboData.ComboableAttackDuration
                + CastTime);
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
    /// Builds the AttackData object used to pass data to the methods handling the attack.
    /// </summary>
    /// <param name="abilityUse">The object containing data about how the ability was used</param>
    /// <returns>The AttackData object</returns>
    private AttackData BuildAttackData(AbilityUse abilityUse)
    {
        AttackData attackData = new();
        attackData.AbilityData = NextComboData.AttackAbilityData;
        attackData.User = UnityUtil.GetParentIfExists(abilityUse.Component.gameObject);
        attackData.Direction = abilityUse.Direction;
        attackData.SetDirectionOnHit = false;
        attackData.EntityData = entityData;
        attackData.AttackEvents.OnAttackSuccessful += AttackSuccessful;
        return attackData;
    }

    /// <summary>
    /// Determines the rotation of the attack object, using the direction. Assumes
    /// the object sprite faces up by default.
    /// </summary>
    /// <param name="direction">The direction of the attack as a Vector2</param>
    /// <returns>The rotation as a Quaternion</returns>
    private Quaternion DetermineRotation(Vector2 direction)
    {
        Quaternion rotation;
        if (NextComboData.PrefabAbilityData.RotatePrefab)
        {
            float angle = Vector2.SignedAngle(Vector2.right, direction) - 90f;
            rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            rotation = Quaternion.identity;
        }
        return rotation;
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
