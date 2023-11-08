using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Component for a pickupable item.
/// </summary>
public class ItemPickup : MonoBehaviour
{
    [SerializeField]
    private Item item;
    [SerializeField]
    private int amount;
    [SerializeField]
    private float duration = 0;
    [SerializeField]
    private Sound pickupSound;
    [SerializeField]
    private TextMeshProUGUI quantityText;

    private bool initialized = false;
    private float timer = 0;
    private bool flashing = false;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (!initialized && item != null)
        {
            UpdateSprite(item);
            initialized = true;
        }
    }

    public void Init(Item item, int amount, float duration = 0)
    {
        this.item = item;
        this.amount = amount;
        this.duration = duration;

        UpdateSprite(item);

        initialized = true;
    }

    private void Update()
    {
        if (duration > 0)
        {
            timer += Time.deltaTime;
            if (timer >= duration)
            {
                Destroy(this.gameObject);
            }
            else if (timer >= duration * 0.75f && !flashing)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
                flashing = true;
                InvokeRepeating(nameof(Flash), 0, 0.2f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (initialized && collision.CompareTag("Player"))
        {
            Inventory inventory = collision.GetComponent<Inventory>();
            if (inventory != null)
            {
                inventory.AddItem(item, amount);
                AudioManager.Instance.Play(pickupSound);
                Destroy(this.gameObject);
            }
        }
    }

    private void UpdateSprite(Item newItem)
    {
        spriteRenderer.sprite = newItem.Icon;
        spriteRenderer.enabled = true;
        if (quantityText != null && amount > 1)
        {
            quantityText.text = amount.ToString();
        }
    }

    private void Flash()
    {
        spriteRenderer.enabled = !(spriteRenderer.enabled);
    }
}
