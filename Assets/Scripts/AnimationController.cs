using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component responsible for animating an entity.
/// </summary>
public class AnimationController : MonoBehaviour
{
    private EntityState entityState;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        entityState = GetComponent<EntityController>().EntityState;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLookDirection();
        UpdateIsMoving();
    }

    /// <summary>
    /// Updates the look direction on the Animator, from the EntityState.
    /// </summary>
    private void UpdateLookDirection()
    {
        animator.SetFloat("xDirection", entityState.LookDirection.x);
        animator.SetFloat("yDirection", entityState.LookDirection.y);
    }

    /// <summary>
    /// Updates the Animator isMoving property, from the EntityState's Action.
    /// </summary>
    private void UpdateIsMoving()
    {
        if (entityState.Action == Action.Move)
        {
            animator.SetBool("isMoving", true);
        } else
        {
            animator.SetBool("isMoving", false);
        }
    }
}
