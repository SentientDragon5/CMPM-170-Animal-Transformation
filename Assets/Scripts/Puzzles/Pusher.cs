using UnityEngine;
using UnityEngine.InputSystem;

public class Pusher : MonoBehaviour
{
    public PlayerInput playerInput;
    public Rigidbody rb;
    public FixedJoint joint;

    [Header("Raycasting")]
    public float rayHeightOffset = 1f;
    public LayerMask interactableMask = 512; // 2 ^ 9
    public float maxGrabDist = 0.5f;


    void Start()
    {
        rb = transform.parent.GetComponent<Rigidbody>();
        var grabbing = playerInput.actions["Grab"].ReadValue<float>() > 0.1f;
    }

    void OnEnable()
    {
        playerInput.actions["Grab"].started += OnGrab;
        playerInput.actions["Grab"].canceled += OnRelease;
        playerInput.actions["Grab"].Enable();
    }
    void OnDisable()
    {
        playerInput.actions["Grab"].started -= OnGrab;
        playerInput.actions["Grab"].canceled -= OnRelease;
        playerInput.actions["Grab"].Disable();
    }


    void OnRelease(InputAction.CallbackContext context)
    {
        joint.connectedBody = null;
    }
    void OnGrab(InputAction.CallbackContext context)
    {
        var grabbable = GetGrabbable();
        joint.connectedBody = grabbable;
    }

    public Rigidbody GetGrabbable()
    {
        var origin = rb.transform.position + Vector3.up * rayHeightOffset;

        if (Physics.Raycast(origin, rb.transform.forward, out RaycastHit hit, maxGrabDist, interactableMask))
        {
            if (hit.collider.TryGetComponent(out Pushable p))
            {
                return hit.rigidbody;
            }
        }
        return null;
    }

    void Update()
    {
        
    }
}
