using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for an object to be destroyed after a specified amount of time has passed.
/// </summary>
public class DestroyTimer : MonoBehaviour
{
    [field: SerializeField]
    public float Duration { get; set; }

    [SerializeField]
    private float maxTimer = 180;
    private float timer = 0;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

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
            if (animator != null)
            {
                animator.SetTrigger("disappear");
                Destroy(gameObject, 1.0f);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
