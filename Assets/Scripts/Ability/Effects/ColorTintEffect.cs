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

    private static Color OriginalColor = Color.white;

    public override void Trigger(EffectData effectData)
    {
        effectData.SpriteRenderer.color = tintColor;
    }

    public override void Unapply(EffectData effectData)
    {
        effectData.SpriteRenderer.color = OriginalColor;
    }
}
