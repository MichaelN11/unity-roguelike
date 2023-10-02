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
    private Spawner destroyWhenKilled;
    public Spawner DestroyWhenKilled => destroyWhenKilled;

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
        destroyWhenKilled.OnSpawn += TriggerTargetSpawned;
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
    }

    private void Update()
    {
        if (player != null
            && triggerBounds != null
            && readyToTrigger
            && triggerBounds.Contains(player.transform.position))
        {
            spriteRenderer.enabled = true;
            animator.enabled = true;
            colliderComponent.enabled = true;
            readyToTrigger = false;
        }
    }

    private void TriggerTargetSpawned(GameObject target)
    {
        target.GetComponent<EntityState>().OnDeath += TriggerTargetDeath;
        readyToTrigger = true;
    }

    private void TriggerTargetDeath()
    {
        colliderComponent.enabled = false;
        animator.SetTrigger("destroy");
        Destroy(this.gameObject, 5);
    }
}
