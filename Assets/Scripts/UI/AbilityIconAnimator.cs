using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for managing animating multiple ability icons in the UI.
/// </summary>
public class AbilityIconAnimator : MonoBehaviour
{
    [SerializeField]
    private List<RectTransform> abilityIcons = new();

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

    private Camera mainCamera;
    private Canvas uiCanvas;

    private Vector2 startPosition;
    private Vector2 targetPosition;
    private bool isAnimating = false;
    private float timer;
    private RectTransform movingIconRect;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        uiCanvas = UIController.Instance.GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAnimating)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / duration);

            // Curved interpolation
            float moveT = moveCurve.Evaluate(t);
            float scaleT = scaleCurve.Evaluate(t);

            // Move toward target
            movingIconRect.anchoredPosition = Vector2.LerpUnclamped(startPosition, targetPosition, moveT);

            // Scale with overshoot
            float scale = Mathf.LerpUnclamped(startScale, 1f, scaleT);
            movingIconRect.localScale = Vector3.one * scale;

            if (t >= 1f)
            {
                Destroy(movingIconRect.gameObject);
                isAnimating = false;
            }
        }
    }

    public void StartMovingIconAnimation(Vector2 startWorldPosition, int abilityNumber, Sprite sprite)
    {
        if (abilityNumber < abilityIcons.Count)
        {
            startPosition = WorldToCanvasPosition(startWorldPosition);
            targetPosition = ScreenToCanvasPosition(abilityIcons[abilityNumber].position);
            isAnimating = true;
            timer = 0;
            GameObject movingIcon = Instantiate(movingIconPrefab, uiCanvas.transform, false);
            movingIconRect = movingIcon.GetComponent<RectTransform>();
            movingIconRect.anchoredPosition = startPosition;

            Image iconImage = movingIcon.GetComponent<Image>();
            iconImage.sprite = sprite;
        }
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
