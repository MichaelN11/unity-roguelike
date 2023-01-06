using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for handling player input for the entity.
/// </summary>
public class PlayerInput : MonoBehaviour
{
    private EntityController entityController;

    // Start is called before the first frame update
    void Start()
    {
        entityController = GetComponent<EntityController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (entityController != null)
        {
            entityController.SetMovementDirection(DetermineMovementDirection());
            entityController.SetLookDirection(DetermineLookDirection());
        }
    }

    /// <summary>
    /// Determines the movement direction from the input axises.
    /// </summary>
    /// <returns>The direction the player should move</returns>
    private Vector2 DetermineMovementDirection()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        return new Vector2(horizontal, vertical);
    }

    /// <summary>
    /// Determines the direction the player is looking in, using the mouse position
    /// relative to the player position.
    /// </summary>
    /// <returns>The direction the player is looking in</returns>
    private Vector2 DetermineLookDirection()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 entityPosition = transform.position;
        Vector2 mousePositionRelativeToEntity = new(
            mousePosition.x - entityPosition.x,
            mousePosition.y - entityPosition.y);
        return mousePositionRelativeToEntity;
    }
}
