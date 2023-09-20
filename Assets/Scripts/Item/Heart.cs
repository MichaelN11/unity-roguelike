using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for a heart item that heals the player when picked up.
/// </summary>
public class Heart : MonoBehaviour
{
    [SerializeField]
    private float healAmount = 1;

    [SerializeField]
    private Sound soundEffect;

    [SerializeField]
    private float duration = 0;

    private SpriteRenderer spriteRenderer;
    private float timer = 0;
    private bool flashing = false;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= duration)
        {
            Destroy(this.gameObject);
        } else if (timer >= duration * 0.75f && !flashing)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            flashing = true;
            InvokeRepeating(nameof(Flash), 0, 0.2f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Damageable damageable = collision.GetComponent<Damageable>();
            if (damageable != null && damageable.CurrentHealth < damageable.MaxHealth)
            {
                AudioManager.Instance.Play(soundEffect);
                damageable.Heal(healAmount);
                Destroy(this.gameObject);
            }
        }
    }

    private void Flash()
    {
        spriteRenderer.enabled = !(spriteRenderer.enabled);
    }
}
