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
    private bool isFireHeld = false;

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
        inputActions.Player.Move.performed += OnMovePerformed;
        inputActions.Player.Move.canceled += OnMoveCancelled;
        inputActions.Player.Look.performed += OnLook;

        inputActions.UI.Restart.performed += OnRestart;
        inputActions.UI.Quit.performed += OnQuit;
    }

    private void Update()
    {
        if (isFireHeld)
        {
            FireHeld();
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

    /// <summary>
    /// Called by the Input System. Tried to attack, and sets the attack buffer if the attack failed.
    /// </summary>
    /// <param name="ctx">The callback context</param>
    public void OnFireStarted(CallbackContext ctx)
    {
        isFireHeld = true;
        InputData inputData = new();
        inputData.Type = InputType.Attack;
        inputData.Direction = lookDirection;
        bool updateSuccessful = entityController.UpdateFromInput(inputData);
        if (!updateSuccessful)
        {
            bufferedInput = inputData;
            bufferTimer = InputBuffer;
        }
    }

    /// <summary>
    /// Called by the Input System. Sets fire held to false.
    /// </summary>
    /// <param name="ctx">The callback context</param>
    public void OnFireCancelled(CallbackContext ctx)
    {
        isFireHeld = false;
    }

    /// <summary>
    /// Actions taken while the fire button is held. Triggers an attack action for the EntityController.
    /// Buffers the input so it can be used when entity can next act.
    /// </summary>
    private void FireHeld()
    {
        InputData inputData = new();
        inputData.Type = InputType.Attack;
        inputData.Direction = lookDirection;
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

    /// <summary>
    /// Reloads the current scene when the restart key is pressed.
    /// </summary>
    /// <param name="ctx">The callback context</param>
    private void OnRestart(CallbackContext ctx)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("Done restarting");
    }

    /// <summary>
    /// Quits the game when the quit key is pressed.
    /// </summary>
    /// <param name="ctx">The callback context</param>
    private void OnQuit(CallbackContext ctx)
    {
        Application.Quit();
    }
}
