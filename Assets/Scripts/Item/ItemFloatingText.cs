using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Component for the floating text displaying an acquired item.
/// </summary>
public class ItemFloatingText : MonoBehaviour
{
    private const float Acceleration = -2f;
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
            UpdateSpeed(Acceleration * Time.deltaTime * 0.5f);
            float newYPosition = transform.position.y + (currentSpeed * Time.deltaTime);
            transform.position = new(transform.position.x, newYPosition);
            UpdateSpeed(Acceleration * Time.deltaTime * 0.5f);
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

    private void UpdateSpeed(float acceleration)
    {
        currentSpeed += acceleration;
        if (currentSpeed < 0)
        {
            currentSpeed = 0;
        }
    }
}
