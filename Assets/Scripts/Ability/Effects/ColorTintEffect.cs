using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ability effect that tints the sprite of the entity.
/// </summary>
[CreateAssetMenu(menuName = "Game Data/Ability Effect/Color Tint")]
public class ColorTintEffect : AbilityEffect
{
    [SerializeField]
    private Color tintColor;
    public Color TintColor => tintColor;

    [SerializeField]
    private float duration;
    public float Duration => duration;

    private IEnumerator coroutine;

    public override void Trigger(EffectData effectData)
    {
        Color originalColor = effectData.SpriteRenderer.color;
        effectData.SpriteRenderer.color = tintColor;

        coroutine = ResetColor(effectData.SpriteRenderer, originalColor);
        effectData.AbilityManager.StartCoroutine(coroutine);
    }

    private IEnumerator ResetColor(SpriteRenderer spriteRenderer, Color originalColor)
    {
        yield return new WaitForSeconds(duration);
        spriteRenderer.color = originalColor;
    }
}
