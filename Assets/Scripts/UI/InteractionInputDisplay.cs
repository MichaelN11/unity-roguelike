using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for the input key sprite that displays over an interactable object when it is in range of the player.
/// </summary>
public class InteractionInputDisplay : MonoBehaviour
{
    [SerializeField]
    private float yOffsetFromInteractable = 1.25f;

    private EntityController playerEntity;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (playerEntity == null)
        {
            if (PlayerController.Instance != null)
            {
                playerEntity = PlayerController.Instance.GetComponent<EntityController>();
            }
            return;
        }

        if (playerEntity.CurrentInteractableObject != null)
        {
            spriteRenderer.enabled = true;
            Vector2 interactablePosition = playerEntity.CurrentInteractableObject.transform.position;
            transform.position = new Vector2(interactablePosition.x, interactablePosition.y + yOffsetFromInteractable);
        } else
        {
            spriteRenderer.enabled = false;
        }
    }
}
