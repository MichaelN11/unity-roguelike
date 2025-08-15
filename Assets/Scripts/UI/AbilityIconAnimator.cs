using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for animating ability icons in the UI. Currently only supports animating one at a time.
/// </summary>
public class AbilityIconAnimator : MonoBehaviour
{
    [SerializeField]
    private List<AbilityIcon> abilityIcons = new();

    [Tooltip("Controls movement easing")]
    [SerializeField]
    private AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Tooltip("Controls scale with overshoot")]
    [SerializeField]
    private AnimationCurve scaleCurve = new(
        new Keyframe(0f, 0f),       // start at 0
        new Keyframe(0.8f, 1.15f),  // overshoot slightly before the end
        new Keyframe(1f, 1f)        // settle at normal size
    );

    [SerializeField]
    private float duration = 1f;         // Time to move
    [SerializeField]
    private float startScale = 0.3f;     // Initial size factor

    [SerializeField]
    private GameObject movingIconPrefab;

    [SerializeField]
    private Sound startSound;
    [SerializeField]
    private Sound endSound;

    [SerializeField]
    private int canvasSiblingIndex = 5;

    private Camera mainCamera;
    private Canvas uiCanvas;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        uiCanvas = UIController.Instance.GetComponent<Canvas>();
    }

    public void StartMovingIconAnimation(Vector2 startWorldPosition, int abilityNumber, Sprite sprite)
    {
        if (abilityNumber < abilityIcons.Count)
        {
            AudioManager.Instance.Play(startSound);

            Vector2 startPosition = WorldToCanvasPosition(startWorldPosition);
            Vector2 targetPosition = ScreenToCanvasPosition(abilityIcons[abilityNumber].GetComponent<RectTransform>().position);
            GameObject movingIcon = Instantiate(movingIconPrefab, uiCanvas.transform, false);
            movingIcon.transform.SetSiblingIndex(canvasSiblingIndex);
            RectTransform movingIconRect = movingIcon.GetComponent<RectTransform>();
            movingIconRect.anchoredPosition = startPosition;

            Image iconImage = movingIcon.GetComponent<Image>();
            iconImage.sprite = sprite;
            movingIconRect.localScale = Vector3.one * startScale;
            iconImage.enabled = true;

            IEnumerator coroutine = AnimationCoroutine(movingIconRect, startPosition, targetPosition, abilityNumber);
            StartCoroutine(coroutine);
        }
    }

    private IEnumerator AnimationCoroutine(RectTransform movingIconRect, Vector2 startPosition, Vector2 targetPosition, int abilityNumber)
    {
        float timer = 0;
        float t = 0;

        while (t < 1)
        {
            timer += Time.deltaTime;

            t = Mathf.Clamp01(timer / duration);

            // Curved interpolation
            float moveT = moveCurve.Evaluate(t);
            float scaleT = scaleCurve.Evaluate(t);

            // Move toward target
            movingIconRect.anchoredPosition = Vector2.LerpUnclamped(startPosition, targetPosition, moveT);

            // Scale with overshoot
            float scale = Mathf.LerpUnclamped(startScale, 1f, scaleT);
            movingIconRect.localScale = Vector3.one * scale;

            yield return null;
        }

        Destroy(movingIconRect.gameObject);
        abilityIcons[abilityNumber].Show();
        AudioManager.Instance.Play(endSound);
    }

    private Vector2 WorldToCanvasPosition(Vector2 worldPos)
    {
        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);
        return ScreenToCanvasPosition(screenPos);
    }

    private Vector2 ScreenToCanvasPosition(Vector2 screenPos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            uiCanvas.transform as RectTransform,
            screenPos,
            uiCanvas.worldCamera,
            out Vector2 localPos
        );
        return localPos;
    }
}
