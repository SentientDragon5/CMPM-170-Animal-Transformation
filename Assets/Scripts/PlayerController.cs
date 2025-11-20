using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;

    public PlayerInput playerInput;

    public List<GenericMoveController> movementControllers;
    public int moveState = 0;

    public List<WaterVolume> waterBodyList;

    void SwitchState(int newState)
    {
        this.waterBodyList.Clear();
        Debug.Log("Switching state to " + newState);
        if (moveState == newState)
            return;

        movementControllers[moveState].Exit();
        moveState = newState;
        movementControllers[moveState].Enter();
    }

    void Start()
    {
        for (int i = 0; i < movementControllers.Count; i++)
        {
            if (i==moveState)
                movementControllers[moveState].Enter();
            else
                movementControllers[i].Exit();
        }
    }

    void OnEnable()
    {
        playerInput.actions["Jump"].performed += Jump;
        playerInput.actions["Jump"].Enable();

        // Number keys for switching the states
        // Lambdas bc im too lazy to write each function
        for (int i = 0; i < 4; i++)
        {
            string actionName = (i + 1).ToString(); // strings correlate to index - 1, so 1 -> 0, 2 -> 1, etc.
            int moveIndex = (int)i; // copy by value
            playerInput.actions[actionName].performed += (InputAction.CallbackContext context) => SwitchState(moveIndex);
            playerInput.actions[actionName].Enable();
        }
        Cursor.lockState = CursorLockMode.Locked; // Move this later!
    }
    void OnDisable()
    {
        playerInput.actions["Jump"].performed -= Jump;
        playerInput.actions["Jump"].Disable();

        for (int i = 0; i < 4; i++)
        {
            string actionName = (i + 1).ToString(); // strings correlate to index - 1, so 1 -> 0, 2 -> 1, etc.
            int moveIndex = (int)i; // copy by value
            playerInput.actions[actionName].performed -= (InputAction.CallbackContext context) => SwitchState(moveIndex);
            playerInput.actions[actionName].Disable();
        }
        Cursor.lockState = CursorLockMode.None;
    }

    void Jump(InputAction.CallbackContext callbackContext) => movementControllers[moveState].JumpAction();

    void Update()
    {
        Vector2 moveInput2d = playerInput.actions["Move"].ReadValue<Vector2>();
        float moveInputZ = playerInput.actions["MoveZ"].ReadValue<float>();
        Vector3 moveInput = new Vector3(moveInput2d.x, moveInput2d.y, moveInputZ);

        Transform camTransform = Camera.main.transform;
        // local camera forward and right, but global up
        Vector3 camForward = new Vector3(camTransform.forward.x, 0, camTransform.forward.z).normalized;
        Vector3 camRight = new Vector3(camTransform.right.x, 0, camTransform.right.z).normalized;
        Vector3 move = moveInput.y * camForward + moveInput.x * camRight + moveInput.z * Vector3.up;
        if (move.magnitude > 1)
            move.Normalize();

        movementControllers[moveState].Move(move);
    }

}
