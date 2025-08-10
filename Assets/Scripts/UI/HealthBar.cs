using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Component for managing a health bar that displays on the UI. i.e for the player or for a boss.
/// </summary>
public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private bool isPlayer;

    [SerializeField]
    private bool isBoss;

    [SerializeField]
    private TextMeshProUGUI labelText;

    [SerializeField]
    private float widthIncreaseWithHealthIncrease = 30;

    [SerializeField]
    private float animationDuration = 2;

    private Slider slider;
    private Damageable damageable;
    private EntityData entityData;
    private RectTransform rectTransform;
    private float initialWidth;
    private bool updatingWidth = false;

    private void OnEnable()
    {
        if (labelText != null)
        {
            labelText.gameObject.SetActive(true);
        }
    }

    private void Start()
    {
        slider = GetComponent<Slider>();
        rectTransform = slider.GetComponent<RectTransform>();
        initialWidth = rectTransform.sizeDelta.x;
    }

    private void Update()
    {
        if (damageable == null)
        {
            if (isPlayer && PlayerController.Instance != null)
            {
                damageable = PlayerController.Instance.GetComponent<Damageable>();
                entityData = PlayerController.Instance.GetComponent<EntityData>();
            } else if (isBoss && GameManager.Instance.CurrentBoss != null)
            {
                Debug.Log("Found boss: " + GameManager.Instance.CurrentBoss.Entity.Description.ToString());
                damageable = GameManager.Instance.CurrentBoss.GetComponent<Damageable>();
                labelText.text = GameManager.Instance.CurrentBoss.Entity.Description.ToString();
            } else if (isBoss)
            {
                Debug.Log("Hiding boss health bar");
                labelText.gameObject.SetActive(false);
                gameObject.SetActive(false);
            }
        }
        else
        {
            if (!updatingWidth)
            {
                float healthPercentage = damageable.CurrentHealth / damageable.MaxHealth;
                slider.value = healthPercentage;
                if (isPlayer)
                {
                    int maxHealthIncreases = (int)(damageable.MaxHealth - entityData.Entity.MaxHealth);
                    float desiredWidth = initialWidth + (maxHealthIncreases * widthIncreaseWithHealthIncrease);
                    if (rectTransform.sizeDelta.x != desiredWidth)
                    {
                        Debug.Log("updating health bar width sizex: " + rectTransform.sizeDelta.x + " desiredWidth: " + desiredWidth);
                        StopAllCoroutines();
                        updatingWidth = true;
                        StartCoroutine(AnimateWidthCoroutine(desiredWidth));
                    }
                }
            }
        }
    }

    private IEnumerator AnimateWidthCoroutine(float targetWidth)
    {
        Vector2 startSize = rectTransform.sizeDelta;
        float startHealth = damageable.CurrentHealth;

        float startValue = slider.value;
        // Compute desired fill pixel position from left edge
        float targetFillPixel = targetWidth * (startValue - slider.minValue) / (slider.maxValue - slider.minValue);

        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);
            float currentWidth = Mathf.Lerp(startSize.x, targetWidth, t);
            rectTransform.sizeDelta = new Vector2(currentWidth, startSize.y);

            if (damageable.CurrentHealth != startHealth)
            {
                startValue = damageable.CurrentHealth / damageable.MaxHealth;
                targetFillPixel = targetWidth * (startValue - slider.minValue) / (slider.maxValue - slider.minValue);
            }

            // Compute new normalized value so fill stays in same pixel spot
            float normalizedValue = targetFillPixel / currentWidth;
            slider.value = Mathf.Lerp(slider.minValue, slider.maxValue, normalizedValue);

            yield return null;
        }
        rectTransform.sizeDelta = new Vector2(targetWidth, startSize.y);
        updatingWidth = false;
    }
}
