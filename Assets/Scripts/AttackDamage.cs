using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for an object that is created by an Attack. Should have its AttackData
/// set by the attack that generates the object. The object lasts for a limited duration
/// before it is destroyed.
/// </summary>
public class AttackDamage : MonoBehaviour
{
    public AttackData AttackData { get; set; }

    [SerializeField]
    private float maxTimer = 180;
    private float timer = 0;

    private void Update()
    {
        incrementTimer();
    }

    /// <summary>
    /// Increments the timer using deltaTime, and checks to see if the timer is over
    /// the attack duration or the maximum time before destroying the object.
    /// </summary>
    private void incrementTimer()
    {
        timer += Time.deltaTime;
        if ((AttackData != null && timer >= AttackData.duration)
            || timer >= maxTimer)
        {
            Destroy(gameObject);
        }
    }
}
