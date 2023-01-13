using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Singleton component for handling player input for the entity, using the Input System.
/// </summary>
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    private EntityController entityController;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        entityController = GetComponent<EntityController>();
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    /// <summary>
    /// Called by the Input System. Sets the movement direction using the Vector2 from the InputValue.
    /// </summary>
    /// <param name="inputValue">The InputValue containing a Vector2 direction to move in</param>
    public void OnMove(InputValue inputValue)
    {
        Vector2 moveDirection = inputValue.Get<Vector2>();
        InputData inputData = new();
        inputData.Type = InputType.Move;
        inputData.Direction = moveDirection;
        entityController.UpdateFromInput(inputData);
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
        Vector2 inputPositionRelativeToEntity = inputWorldPosition - entityPosition;

        InputData inputData = new();
        inputData.Type = InputType.Look;
        inputData.Direction = inputPositionRelativeToEntity;
        entityController.UpdateFromInput(inputData);
    }

    /// <summary>
    /// Called by the Input System. Triggers an attack action for the EntityController.
    /// </summary>
    /// <param name="inputValue">The InputValue</param>
    public void OnFire(InputValue inputValue)
    {
        InputData inputData = new();
        inputData.Type = InputType.Attack;
        entityController.UpdateFromInput(inputData);
    }
}
