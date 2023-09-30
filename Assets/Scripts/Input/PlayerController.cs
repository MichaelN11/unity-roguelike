using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;

/// <summary>
/// Singleton component for handling player input for the entity, using the Input System.
/// </summary>
public class PlayerController : MonoBehaviour
{
    private const float InputBuffer = 0.2f;

    public static PlayerController Instance { get; private set; }

    private PlayerInputActions inputActions;
    private EntityController entityController;
    private Vector2 lookDirection = Vector2.down;
    private InputData bufferedInput = null;
    private float bufferTimer = 0f;
    /// <summary>
    /// List of ability numbers currently being held down, in order of when they were pressed.
    /// </summary>
    private readonly List<int> heldAbilities = new();

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

        inputActions = new PlayerInputActions();
        inputActions.Enable();
        inputActions.Player.Fire.started += OnFireStarted;
        inputActions.Player.Fire.canceled += OnFireCancelled;
        inputActions.Player.SecondaryFire.started += OnSecondaryFireStarted;
        inputActions.Player.SecondaryFire.canceled += OnSecondaryFireCancelled;

        inputActions.Player.Move.performed += OnMovePerformed;
        inputActions.Player.Move.canceled += OnMoveCancelled;
        inputActions.Player.Look.performed += OnLook;
    }

    private void Update()
    {
        if (heldAbilities.Count != 0)
        {
            AbilityHeld();
        }
        if (bufferedInput != null && bufferTimer > 0)
        {
            bool updateSuccessful = entityController.UpdateFromInput(bufferedInput);
            if (updateSuccessful)
            {
                bufferTimer = 0;
                bufferedInput = null;
            } else
            {
                bufferTimer -= Time.deltaTime;
            }
        }
    }

    private void OnDestroy()
    {
        Instance = null;
        inputActions.Dispose();
    }

    /// <summary>
    /// Called by the Input System. Sets the movement direction using the Vector2 from the InputValue.
    /// </summary>
    /// <param name="ctx">The CallbackContext containing a Vector2 direction to move in</param>
    public void OnMovePerformed(CallbackContext ctx)
    {
        Vector2 moveDirection = ctx.ReadValue<Vector2>();
        SendMoveInput(moveDirection);
    }

    /// <summary>
    /// Called by the Input System. Sets the movement direction to zero.
    /// </summary>
    /// <param name="ctx">The CallbackContext</param>
    public void OnMoveCancelled(CallbackContext ctx)
    {
        SendMoveInput(Vector2.zero);
    }

    /// <summary>
    /// Called by the Input System. Sets the look direction using the Vector2 from the InputValue,
    /// determining the input position relative to the transform position.
    /// </summary>
    /// <param name="ctx">The CallbackContext containing a Vector2 look position</param>
    public void OnLook(CallbackContext ctx)
    {
        Vector2 inputScreenPosition = ctx.ReadValue<Vector2>();
        Vector2 inputWorldPosition = Camera.main.ScreenToWorldPoint(inputScreenPosition);
        Vector2 entityPosition = transform.position;
        Vector2 inputPositionRelativeToEntity = inputWorldPosition - entityPosition;

        InputData inputData = new();
        inputData.Type = InputType.Look;
        inputData.Direction = inputPositionRelativeToEntity;
        entityController.UpdateFromInput(inputData);

        lookDirection = inputPositionRelativeToEntity;
    }

    public void OnFireStarted(CallbackContext ctx)
    {
        AbilityStarted(0);
    }

    public void OnFireCancelled(CallbackContext ctx)
    {
        AbilityCanceled(0);
    }

    public void OnSecondaryFireStarted(CallbackContext ctx)
    {
        AbilityStarted(1);
    }

    public void OnSecondaryFireCancelled(CallbackContext ctx)
    {
        AbilityCanceled(1);
    }

    private void AbilityStarted(int abilityNumber)
    {
        heldAbilities.Add(abilityNumber);

        InputData inputData = new();
        inputData.Type = InputType.Ability;
        inputData.Direction = lookDirection;
        inputData.AbilityNumber = abilityNumber;
        bool updateSuccessful = entityController.UpdateFromInput(inputData);
        if (!updateSuccessful)
        {
            bufferedInput = inputData;
            bufferTimer = InputBuffer;
        }
    }

    private void AbilityCanceled(int abilityNumber)
    {
        heldAbilities.Remove(abilityNumber);
    }

    private void AbilityHeld()
    {
        int currentAbility = heldAbilities[^1];

        InputData inputData = new();
        inputData.Type = InputType.Ability;
        inputData.Direction = lookDirection;
        inputData.AbilityNumber = currentAbility;
        entityController.UpdateFromInput(inputData);
    }

    /// <summary>
    /// Sends the move input to the entity controller.
    /// </summary>
    /// <param name="moveDirection">The Vector2 move direction</param>
    private void SendMoveInput(Vector2 moveDirection)
    {
        InputData inputData = new();
        inputData.Type = InputType.Move;
        inputData.Direction = moveDirection;
        entityController.UpdateFromInput(inputData);
    }
}
