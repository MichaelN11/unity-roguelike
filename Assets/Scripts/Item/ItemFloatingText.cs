using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Component for the floating text displaying an acquired item.
/// </summary>
public class ItemFloatingText : MonoBehaviour
{
    private const float Acceleration = -0.02f;
    private const float InitialSpeed = 2.5f;
    private const float DestroyTime = 3f;

    [SerializeField]
    private TextMeshProUGUI itemText;

    private SpriteRenderer spriteRenderer;

    private bool move = false;
    private float currentSpeed = InitialSpeed;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (move)
        {
            float newYPosition = transform.position.y + (currentSpeed * Time.deltaTime);
            transform.position = new(transform.position.x, newYPosition);
            currentSpeed += Acceleration;
            if (currentSpeed < 0)
            {
                currentSpeed = 0;
            }
        }
    }

    public void Init(InventoryItem inventoryItem)
    {
        itemText.text = inventoryItem.Item.ItemName + " x" + inventoryItem.Amount;
        spriteRenderer.sprite = inventoryItem.Item.Icon;
        spriteRenderer.enabled = true;
        move = true;
        Destroy(gameObject, DestroyTime);
    }
}
