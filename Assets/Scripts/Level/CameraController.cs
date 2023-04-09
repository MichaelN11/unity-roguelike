using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for controlling the camera. Follows the target.
/// </summary>
public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform followTarget;

    [SerializeField]
    private float xSmoothing = 0.125f;

    [SerializeField]
    private float ySmoothing = 0.125f;

    [SerializeField]
    private float xDeadzone = 2;

    [SerializeField]
    private float yDeadzone = 1;

    private IEnumerator updateCoroutine;
    private LevelBounds levelBounds;
    private Camera mainCamera;

    private void Awake()
    {
        updateCoroutine = LateFixedUpdate();
        StartCoroutine(updateCoroutine);
        mainCamera = GetComponent<Camera>();
    }

    private void Start()
    {
        levelBounds = LevelManager.Instance.LevelBounds;
        if (followTarget == null)
        {
            followTarget = PlayerController.Instance.transform;
        }

        float initialX = followTarget.position.x;
        float initialY = followTarget.position.y;
        transform.position = ClampToBounds(new(initialX, initialY, transform.position.z));
    }

    private IEnumerator LateFixedUpdate()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();

            float xPosition = LerpPosition(transform.position.x, followTarget.position.x, xDeadzone, xSmoothing);
            float yPosition = LerpPosition(transform.position.y, followTarget.position.y, yDeadzone, ySmoothing);

            transform.position = ClampToBounds(new(xPosition, yPosition, transform.position.z));
        }
    }

    private float LerpPosition(float position, float targetPosition, float deadzone, float smoothing)
    {
        float newPosition = position;
        if (targetPosition - position > deadzone)
        {
            newPosition = targetPosition - deadzone;
        } else if (position - targetPosition > deadzone) {
            newPosition = targetPosition + deadzone;
        }
        return Mathf.Lerp(position, newPosition, smoothing);
    }

    private Vector3 ClampToBounds(Vector3 cameraPosition)
    {
        Vector3 clampedPosition = cameraPosition;

        float halfHeight = mainCamera.orthographicSize;
        float halfWidth = mainCamera.aspect * halfHeight;

        if (levelBounds != null)
        {
            Bounds bounds = levelBounds.Bounds;
            clampedPosition.x = Mathf.Clamp(cameraPosition.x, bounds.min.x + halfWidth, bounds.max.x - halfWidth);
            clampedPosition.y = Mathf.Clamp(cameraPosition.y, bounds.min.y + halfHeight, bounds.max.y - halfHeight);
        }

        return clampedPosition;
    }
}
