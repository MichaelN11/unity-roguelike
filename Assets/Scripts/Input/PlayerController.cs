using System;
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
    /// <summary>
    /// List of item numbers currently being held down, in order of when they were pressed.
    /// </summary>
    private readonly List<int> heldItems = new();

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

        inputActions.Player.Item1.started += OnItem1Started;
        inputActions.Player.Item1.canceled += OnItem1Cancelled;
        inputActions.Player.Item2.started += OnItem2Started;
        inputActions.Player.Item2.canceled += OnItem2Cancelled;
        inputActions.Player.Item3.started += OnItem3Started;
        inputActions.Player.Item3.canceled += OnItem3Cancelled;

        inputActions.Player.Interact.started += OnInteractStarted;

        inputActions.Player.Move.performed += OnMovePerformed;
        inputActions.Player.Move.canceled += OnMoveCancelled;
        inputActions.Player.Look.performed += OnLook;
    }

    private void Start()
    {
        entityController.CanInteract = true;
    }

    private void Update()
    {
        if (heldAbilities.Count != 0)
        {
            AbilityHeld();
        }
        if (heldItems.Count != 0)
        {
            ItemHeld();
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
        try
        {
            if (gameObject == null)
            {
                return;
            }
        }
        catch (Exception) { return; }

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

    public void OnInteractStarted(CallbackContext ctx)
    {
        InputData inputData = new();
        inputData.Type = InputType.Interact;
        entityController.UpdateFromInput(inputData);
    }

    public void OnFireStarted(CallbackContext ctx)
    {
        AbilityStarted(0);
    }

    public void OnFireCancelled(CallbackContext ctx)
    {
        AbilityCancelled(0);
    }

    public void OnSecondaryFireStarted(CallbackContext ctx)
    {
        AbilityStarted(1);
    }

    public void OnSecondaryFireCancelled(CallbackContext ctx)
    {
        AbilityCancelled(1);
    }

    public void OnItem1Started(CallbackContext ctx)
    {
        ItemStarted(0);
    }

    public void OnItem1Cancelled(CallbackContext ctx)
    {
        ItemCancelled(0);
    }

    public void OnItem2Started(CallbackContext ctx)
    {
        ItemStarted(1);
    }

    public void OnItem2Cancelled(CallbackContext ctx)
    {
        ItemCancelled(1);
    }

    public void OnItem3Started(CallbackContext ctx)
    {
        ItemStarted(2);
    }

    public void OnItem3Cancelled(CallbackContext ctx)
    {
        ItemCancelled(2);
    }

    private void AbilityStarted(int abilityNumber)
    {
        ActionStarted(abilityNumber, heldAbilities, InputType.Ability);
    }

    private void ItemStarted(int itemNumber)
    {
        ActionStarted(itemNumber, heldItems, InputType.Item);
    }

    private void ActionStarted(int actionNumber, List<int> heldActions, InputType type)
    {
        heldActions.Add(actionNumber);

        InputData inputData = new();
        inputData.Type = type;
        inputData.Direction = lookDirection;
        inputData.Number = actionNumber;
        bool updateSuccessful = entityController.UpdateFromInput(inputData);
        if (!updateSuccessful)
        {
            bufferedInput = inputData;
            bufferTimer = InputBuffer;
        }
    }

    private void AbilityCancelled(int abilityNumber)
    {
        heldAbilities.Remove(abilityNumber);
    }

    private void ItemCancelled(int itemNumber)
    {
        heldItems.Remove(itemNumber);
    }

    private void AbilityHeld()
    {
        ActionHeld(heldAbilities, InputType.Ability);
    }

    private void ItemHeld()
    {
        ActionHeld(heldItems, InputType.Item);
    }

    private void ActionHeld(List<int> heldActions, InputType type)
    {
        int current = heldActions[^1];

        InputData inputData = new();
        inputData.Type = type;
        inputData.Direction = lookDirection;
        inputData.Number = current;
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
