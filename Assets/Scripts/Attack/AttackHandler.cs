using System;
using UnityEngine;

/// <summary>
/// Utility class for handling an attack between two entities.
/// </summary>
public class AttackHandler
{
    /// <summary>
    /// Finds the entity from the hitbox and passes it the attack data for the attack.
    /// </summary>
    /// <param name="attackData"></param>
    /// <param name="targetBody"></param>
    public static void AttackEntity(AttackData attackData, Rigidbody2D attackerBody, Rigidbody2D targetBody)
    {
        if (attackData.SetDirectionOnHit)
        {
            attackData.Direction = GetAttackDirection(attackerBody, targetBody);
        }

        Damageable otherDamageable = targetBody.gameObject.GetComponentInParent<Damageable>();
        if (otherDamageable != null
            && IsValidAttackTarget(attackData, targetBody.gameObject, otherDamageable.EntityData))
        {
            otherDamageable.HandleIncomingAttack(attackData);
            if (attackData.AttackEvents != null)
            {
                attackData.AttackEvents.InvokeAttackSuccessful(attackData);
            }
        }
    }

    /// <summary>
    /// Gets the attack direction from the collision, using the position of the rigidbodies.
    /// </summary>
    /// <param name="attackerBody"></param>
    /// <param name="targetBody">The Rigidbody2D object</param>
    /// <returns>The attack direction</returns>
    private static Vector2 GetAttackDirection(Rigidbody2D attackerBody, Rigidbody2D targetBody)
    {
        Vector2 direction = Vector2.zero;
        if (targetBody != null)
        {
            Vector2 otherPosition = targetBody.position;
            Vector2 thisPosition = attackerBody.position;
            direction = otherPosition - thisPosition;
        }
        return direction;
    }

    /// <summary>
    /// Determines if the passed entity is a valid attack target for the attack.
    /// </summary>
    /// <param name="attackData"></param>
    /// <param name="entity">The entity GameObject</param>
    /// <param name="entityData">The EntityData containing data about the entity</param>
    /// <returns>true if the entity is a valid target for the attack</returns>
    private static bool IsValidAttackTarget(AttackData attackData, GameObject entity, EntityData entityData)
    {
        return entity != attackData.User
            && entityData != null
            && attackData.UserEntityData != null
            && attackData.UserEntityData.EnemyFactions.Contains(entityData.Faction);
    }
}
