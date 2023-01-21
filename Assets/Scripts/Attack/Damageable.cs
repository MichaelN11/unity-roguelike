using UnityEngine;

/// <summary>
/// Component for managing health.
/// </summary>
public class Damageable : MonoBehaviour
{
    public float MaxHealth { get; set; }
    public float CurrentHealth { get; private set; }

    private void Start()
    {
        CurrentHealth = MaxHealth;
    }

    /// <summary>
    /// Take damage to health.
    /// </summary>
    /// <param name="damage">The damage being dealt</param>
    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
    }

    /// <summary>
    /// Determines if the object is dead.
    /// </summary>
    /// <returns>true if the object is dead</returns>
    public bool IsDead()
    {
        return CurrentHealth <= 0;
    }
}
