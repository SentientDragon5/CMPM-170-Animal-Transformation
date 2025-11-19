using UnityEngine;

public class CrabController : GenericMoveController
{
    [Header("Walking")]
    public float moveSpeed = 5;
    public float standingTurnSpeed = 200;
    public float movingTurnSpeed = 400;

    [Header("Jumping")]
    public float jumpSpeed = 10;
    public float jumpMinTime = 0.2f;

    [Header("Falling")]
    public float airSpeed = 5;
    public float airControl = 0.1f;
    public float airTurnSpeed = 200;
    public float gravityScale = 2;

    [Header("Grounding")]
    public LayerMask enviromentLayer = 128; // layer 7 so 2 ^ 7
    public float groundingDistance = 0.1f;

    float turnAmount;
    float lastJumpTime;


    // What we want to happen

    // when player is moving, only allow them to move to the moveinput right and left
    // when player is not moving, make the crab face in the same direction as the camera

    // when player moving, no turning
    // when player standing, turning torward camera
    public override void Move(Vector3 moveInput)
    {
        // Convert the input vector to only use X and Z components (ignore Y/vertical input)
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.z);

        // If the input magnitude is greater than 1, normalize it to prevent faster diagonal movement
        if (move.magnitude > 1f) move.Normalize();

        // Convert the movement direction from world space to the player's local space
        // This makes movement relative to the player's current rotation
        move = rb.transform.InverseTransformDirection(move);

        // Check if the player is touching the ground and get the surface normal
        bool grounded = CheckGrounded(out Vector3 normal);

        // Project the movement vector onto the ground plane using the surface normal
        // This prevents sliding down slopes
        move = Vector3.ProjectOnPlane(move, normal);

        // Check if the player is actively providing movement input
        bool isMoving = moveInput.magnitude > 0.1f;

        // Convert to local crab movement - only allow side-to-side movement (X axis)
        // This creates the crab-like movement where you can only strafe left/right
        move = new Vector3(move.x, 0, 0);

        // Convert the local movement back to world space
        move = rb.transform.TransformVector(move);

        // GROUNDED MOVEMENT LOGIC
        if (grounded)
        {
            if (isMoving)
            {
                Debug.Log("Moving - Applying movement and facing camera direction");

                // Get camera forward direction (ignoring Y)
                Transform camTransform = Camera.main.transform;
                Vector3 cameraForward = camTransform.forward;
                cameraForward.y = 0;  // Remove vertical component to keep player upright
                cameraForward.Normalize(); // Ensure consistent length

                // Face camera direction while moving
                if (cameraForward != Vector3.zero)
                {
                    // Create a rotation that looks in the camera's forward direction
                    Quaternion targetRotation = Quaternion.LookRotation(cameraForward, Vector3.up);
                    // Apply the rotation to the rigidbody
                    rb.MoveRotation(targetRotation);
                }

                // Apply movement velocity
                // move.x and move.z now represent world-space movement
                // Only modify X and Z velocity, preserve Y (gravity/vertical) velocity
                rb.linearVelocity = new Vector3(move.x * moveSpeed, rb.linearVelocity.y, move.z * moveSpeed);
            }
            else
            {
                Debug.Log("Not Moving - Rotating to camera");
                // Face camera direction when not moving
                Transform camTransform = Camera.main.transform;
                Vector3 lookDir = camTransform.forward;
                lookDir.y = 0;  // Remove vertical component
                lookDir = lookDir.normalized; // Ensure consistent length

                if (lookDir != Vector3.zero)
                {
                    // Rotate to face the camera direction
                    rb.MoveRotation(Quaternion.LookRotation(lookDir, Vector3.up));
                }

                // Stop horizontal movement when no input is given
                // Only set X and Z to zero, preserve Y velocity (in case of falling/gravity)
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            }
        }
        else // AIR MOVEMENT LOGIC (when not grounded)
        {
            // Preserve the current Y velocity (for jumping/falling)
            move.y = rb.linearVelocity.y;

            // Apply additional gravity force for faster falling
            rb.linearVelocity += gravityScale * Physics.gravity * Time.deltaTime;

            // Gradually move toward the desired air movement velocity
            // This provides limited air control based on input magnitude
            rb.linearVelocity = Vector3.MoveTowards(rb.linearVelocity, move * airSpeed, airControl * moveInput.magnitude);

            // Apply rotation in air (allows some turning while airborne)
            rb.MoveRotation(transform.rotation * Quaternion.AngleAxis(turnAmount * airTurnSpeed * Time.deltaTime, Vector3.up));
        }
    }

    public override void JumpAction()
    {
        if (CheckGrounded(out Vector3 normal))
        {
            lastJumpTime = Time.time;
            rb.linearVelocity += normal * jumpSpeed;
        }
    }

    protected bool CheckGrounded(out Vector3 normal)
    {
        normal = Vector3.zero;
        if (Time.time - lastJumpTime < jumpMinTime)
        {
            Debug.DrawRay(transform.position + Vector3.up * 0.1f, Vector3.down * 0.2f, Color.white);
            return false;
        }

        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit, 0.2f, enviromentLayer))
        {
            Debug.DrawRay(transform.position + Vector3.up * 0.1f, Vector3.down * 0.2f, Color.green);
            normal = hit.normal;
            return true;
        }
        Debug.DrawRay(transform.position + Vector3.up * 0.1f, Vector3.down * 0.2f, Color.red);
        return false;
    }
}
