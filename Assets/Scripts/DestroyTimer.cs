using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for an object to be destroyed after a specified amount of time has passed.
/// </summary>
public class DestroyTimer : MonoBehaviour
{
    public float Duration { get; set; }

    [SerializeField]
    private float maxTimer = 180;
    private float timer = 0;

    private void Update()
    {
        IncrementTimer();
    }

    /// <summary>
    /// Increments the timer using deltaTime, and checks to see if the timer is over
    /// the duration or the maximum time before destroying the object.
    /// </summary>
    private void IncrementTimer()
    {
        timer += Time.deltaTime;
        if (timer >= Duration
            || timer >= maxTimer)
        {
            Destroy(gameObject);
        }
    }
}
