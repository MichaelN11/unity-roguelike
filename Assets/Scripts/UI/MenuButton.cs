using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Component for a menu button that plays a sound when hovered over.
/// </summary>
public class MenuButton : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField]
    private Sound hoverSound;

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.Play(hoverSound);
    }
}
