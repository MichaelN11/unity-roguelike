using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for handling a wall that gets created or destroyed as a result of an event in the game.
/// </summary>
public class EventWall : MonoBehaviour
{
    [SerializeField]
    private GameObject triggerArea;
    public GameObject TriggerArea => triggerArea;

    [SerializeField]
    private Entity destroyWhenKilled;
    public Entity DestroyWhenKilled => destroyWhenKilled;

    [SerializeField]
    private Sound triggerSound;

    [SerializeField]
    private Sound destroySound;

    private GameObject player;
    private Bounds triggerBounds;
    private bool readyToTrigger = false;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Collider2D colliderComponent;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        colliderComponent = GetComponent<Collider2D>();
    }

    private void Start()
    {
        player = PlayerController.Instance.gameObject;
        if (triggerArea != null)
        {
            triggerBounds = new Bounds();
            triggerBounds.SetMinMax(triggerArea.transform.position,
                triggerArea.transform.position + new Vector3(triggerArea.transform.localScale.x, triggerArea.transform.localScale.y));
        } else
        {
            Debug.Log("Trigger area object not set for EventWall.");
        }

        LevelManager.Instance.OnLevelInitialized += FindTargetEntity;
    }

    private void Update()
    {
        if (player != null
            && triggerBounds != null
            && readyToTrigger
            && triggerBounds.Contains(player.transform.position))
        {
            AudioManager.Instance.Play(triggerSound);
            spriteRenderer.enabled = true;
            animator.enabled = true;
            colliderComponent.enabled = true;
            readyToTrigger = false;
        }
    }

    /// <summary>
    /// Find the first entity in the level matching the target Entity type, and subscribe to the death event.
    /// </summary>
    private void FindTargetEntity()
    {
        foreach (EntityData entityData in FindObjectsOfType<EntityData>())
        {
            if (!entityData.CompareTag("Player") && entityData.Entity == destroyWhenKilled)
            {
                entityData.GetComponent<EntityState>().OnDeath += TargetEntityDeath;
                readyToTrigger = true;
                break;
            }
        }
    }

    private void TargetEntityDeath(DeathContext deathContext)
    {
        AudioManager.Instance.Play(destroySound);
        colliderComponent.enabled = false;
        animator.SetTrigger("destroy");
        Destroy(this.gameObject, 5);
    }
}
