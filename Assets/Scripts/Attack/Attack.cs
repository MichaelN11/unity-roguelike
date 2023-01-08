using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component used for using an attack.
/// </summary>
public class Attack : MonoBehaviour
{
    [SerializeField]
    private AttackStats attackStats = new();
    [SerializeField]
    private string attackObjectResourceName = "AttackObject";

    private GameObject attackPrefab;

    private void Start()
    {
        attackPrefab = (GameObject) Resources.Load(attackObjectResourceName);
    }

    /// <summary>
    /// Use the attack. Attacks in the passed direction, at the passed distance.
    /// An AttackDamage object is created with the AttackData set.
    /// </summary>
    /// <param name="direction">The direction of the attack</param>
    /// <param name="distance">The distance away the attack is used</param>
    public void Use(Vector2 direction, float distance)
    {
        float angle = Vector2.SignedAngle(Vector2.right, direction) - 90f;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        distance += attackStats.range;
        Vector3 position = transform.position + (Vector3) direction.normalized * distance;
        GameObject gameObject = Instantiate(attackPrefab, position, rotation);

        AttackObject attackObject = gameObject.GetComponent<AttackObject>();

        AttackData attackData = new();
        attackData.AttackStats = attackStats;
        attackData.User = gameObject;
        attackData.Direction = direction;
        attackObject.AttackData = attackData;
    }
}
