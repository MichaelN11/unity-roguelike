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

    private MeleeAttackComboData NextComboData => meleeAttack.ComboDataList[nextComboStage];

    private EntityData entityData;
    private readonly int numComboStages;
    private int nextComboStage = 0;
    private float comboTimer = 0;
    private float comboableAttackTime = 0;

    public MeleeAttackBehavior(MeleeAttack meleeAttack, AbilityManager abilityManager)
    {
        this.meleeAttack = meleeAttack;
        numComboStages = meleeAttack.ComboDataList.Count();
        abilityManager.UpdateEvent += UpdateAbility;
    }

    public override bool IsUsable(AbilityUse abilityUse)
    {
        return abilityUse.User.CanAct()
            || (abilityUse.User.ActionState == ActionState.UsingAbility
                && abilityUse.User.StunTimer <= comboableAttackTime);
    }

    public override void Interrupt(AbilityManager component)
    {
        ResetCombo();
        base.Interrupt(component);
    }

    protected override void StartAbility(AbilityUse abilityUse)
    {
        entityData = abilityUse.User;

        Vector2 distance = abilityUse.Direction.normalized * NextComboData.AttackAbilityData.Range;
        Vector3 position = abilityUse.Position + distance;
        GameObject instance = Object.Instantiate(NextComboData.PrefabAbilityData.Prefab,
            position,
            DetermineRotation(abilityUse.Direction));

        instance.transform.parent = abilityUse.Component.gameObject.transform;

        DestroyTimer destroyTimer = instance.GetComponent<DestroyTimer>();
        destroyTimer.Duration = NextComboData.PrefabAbilityData.PrefabDuration;

        AttackOnCollision attackObject = instance.GetComponent<AttackOnCollision>();
        attackObject.attackData = BuildAttackData(abilityUse);

        AudioManager.Instance.Play(NextComboData.AttackAbilityData.SoundOnUse);

        UpdateMovement(abilityUse.User);

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
        abilityUse.User.StopMoving();
        abilityUse.User.AttackAnimation = NextComboData.AttackAbilityData.AttackAnimation;
        abilityUse.User.LookDirection = abilityUse.Direction;
        abilityUse.User.ChangeToAbilityState(NextComboData.AttackAbilityData.AttackDuration
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
        attackData.EntityType = abilityUse.EntityType;
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
    /// <param name="direction">The Vector2 direction to move in</param>
    /// <param name="speed">The movement speed</param>
    /// <param name="acceleration">The movement acceleration</param>
    private void UpdateMovement(EntityData entityData)
    {
        entityData.MoveDirection = entityData.LookDirection;
        entityData.MoveSpeed = NextComboData.MovementAbilityData.MoveSpeed;
        entityData.Acceleration = NextComboData.MovementAbilityData.MoveAcceleration;
    }

    /// <summary>
    /// Called after a successful attack.
    /// </summary>
    /// <param name="attackData">The attack data from the successful attack</param>
    private void AttackSuccessful(AttackData attackData)
    {
        AudioManager.Instance.Play(attackData.AbilityData.SoundOnHit);
        entityData.StopTimer = attackData.AbilityData.HitStop;
    }
}
