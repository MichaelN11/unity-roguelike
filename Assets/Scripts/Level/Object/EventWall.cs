using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for handling a wall that gets created or destroyed as a result of an event in the game.
/// </summary>
public class EventWall : MonoBehaviour
{
    [SerializeField]
    private Sound triggerSound;

    [SerializeField]
    private Sound destroySound;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Collider2D colliderComponent;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        colliderComponent = GetComponent<Collider2D>();
    }

    public void TriggerWall()
    {
        AudioManager.Instance.Play(triggerSound);
        spriteRenderer.enabled = true;
        animator.enabled = true;
        colliderComponent.enabled = true;
    }

    public void RetractWall()
    {
        AudioManager.Instance.Play(destroySound);
        colliderComponent.enabled = false;
        animator.SetTrigger("destroy");
        Destroy(this.gameObject, 5);
    }
}
