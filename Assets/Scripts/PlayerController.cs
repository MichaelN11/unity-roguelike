using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Component for handling player input for the entity, using the Input System.
/// </summary>
public class PlayerController : MonoBehaviour
{
    private EntityController entityController;

    // Start is called before the first frame update
    void Start()
    {
        entityController = GetComponent<EntityController>();
    }

    /// <summary>
    /// Called by the Input System. Sets the movement direction using the Vector2 from the InputValue.
    /// </summary>
    /// <param name="inputValue">The InputValue containing a Vector2 direction to move in</param>
    public void OnMove(InputValue inputValue)
    {
        Vector2 moveDirection = inputValue.Get<Vector2>();
        entityController.SetMovementDirection(moveDirection);
    }

    /// <summary>
    /// Called by the Input System. Sets the look direction using the Vector2 from the InputValue,
    /// determining the input position relative to the transform position.
    /// </summary>
    /// <param name="inputValue">The InputValue containing a Vector2 look position</param>
    public void OnLook(InputValue inputValue)
    {
        Vector2 inputScreenPosition = inputValue.Get<Vector2>();
        Vector2 inputWorldPosition = Camera.main.ScreenToWorldPoint(inputScreenPosition);
        Vector2 entityPosition = transform.position;
        Vector2 inputPositionRelativeToEntity = new(
            inputWorldPosition.x - entityPosition.x,
            inputWorldPosition.y - entityPosition.y);
        entityController.SetLookDirection(inputPositionRelativeToEntity);
    }
}
