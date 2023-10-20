using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class holding relevant context about an entity's death.
/// </summary>
public class DeathContext
{
    public EntityDescription KillingEntity { get; set; }
    public AttackDescription KillingAttack { get; set; }
}
