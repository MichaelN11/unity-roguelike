using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component used for using an attack.
/// </summary>
public class Attack : MonoBehaviour
{
    [SerializeField]
    private AttackData attackData = new();
    [SerializeField]
    private string attackObjectResourceName = "AttackObject";

    private GameObject attackObject;

    private void Start()
    {
        attackObject = (GameObject) Resources.Load(attackObjectResourceName);
    }

    /// <summary>
    /// Use the attack. Attacks in the passed direction, at the passed distance.
    /// An AttackDamage object is created with the AttackData set.
    /// </summary>
    /// <param name="direction">The direction of the attack</param>
    /// <param name="distance">The distance away the attack is used</param>
    public void Use(Vector2 direction, float distance)
    {
        var angle = Vector2.SignedAngle(Vector2.right, direction) - 90f;
        var rotation = Quaternion.Euler(0, 0, angle);
        Vector3 position = transform.position + (Vector3) direction.normalized * distance;
        GameObject gameObject = Instantiate(attackObject, position, rotation);

        AttackDamage damageOnCollide = gameObject.GetComponent<AttackDamage>();
        damageOnCollide.AttackData = attackData;
    }
}
