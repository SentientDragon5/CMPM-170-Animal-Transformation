using UnityEngine;

public class HumanoidController : GenericMoveController
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

    public override void Move(Vector3 moveInput)
    {
        // called on update, so we use this to move the player
        // first we take the movement input from player 
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.z);

        if (move.magnitude > 1f) move.Normalize();
        move = rb.transform.InverseTransformDirection(move);

        // Local Space Section
        bool grounded = CheckGrounded(out Vector3 normal);
        move = Vector3.ProjectOnPlane(move, normal);
        turnAmount = Mathf.Atan2(move.x, move.z);

        // if you set move.x to 0, then the character will not be able to walk to the right
        // if you set the turn speed to 0 the character will not be able to turn
        
        // Back to World Space
        move = rb.transform.TransformVector(move);
        
        if (grounded)
        {
            // set the players velocity to the move input * speed
            rb.linearVelocity = move * moveSpeed;
            // decide whether to move with a standing speed or the moving speed
            float turnSpeed = Mathf.Lerp(standingTurnSpeed, movingTurnSpeed, move.magnitude);
            // rotate player by turn
            rb.MoveRotation(transform.rotation * Quaternion.AngleAxis(turnAmount * turnSpeed * Time.deltaTime, Vector3.up));
        }
        else
        {
            // add gravity to our velocity
            // slowly air strafe in a controlled way
            move = Vector3.MoveTowards(rb.linearVelocity, move * airSpeed, airControl * moveInput.magnitude);
            move.y = rb.linearVelocity.y;
            rb.linearVelocity += gravityScale * Physics.gravity * Time.deltaTime;
            // rotate the player by turn amount
            rb.MoveRotation(transform.rotation * Quaternion.AngleAxis(turnAmount * airTurnSpeed * Time.deltaTime, Vector3.up));
        }
    }

    // called by the player controller when the jump input is pressed
    public override void JumpAction()
    {
        // only jump if on the ground
        if (CheckGrounded(out Vector3 normal) && (Time.time - lastJumpTime > jumpMinTime))
        {
            lastJumpTime = Time.time;
            rb.linearVelocity += normal * jumpSpeed;
        }
    }

    // this function gets whether the player is grounded. if so it also gives the normal of the ground, otherwise it gives Vector3.zero
    protected bool CheckGrounded(out Vector3 normal)
    {
        // default normal direction
        normal = Vector3.zero;

        // If we just jumped, we know we are not on the ground. 
        if (Time.time - lastJumpTime < jumpMinTime)
        {
            Debug.DrawRay(transform.position + Vector3.up * groundingDistance/2f, Vector3.down * groundingDistance, Color.white);
            return false;
        }

        // check to see if the ground below us is there, if so also return the normal
        if (Physics.Raycast(transform.position + Vector3.up * groundingDistance/2f, Vector3.down, out RaycastHit hit, groundingDistance, enviromentLayer))
        {
            Debug.DrawRay(transform.position + Vector3.up * groundingDistance/2f, Vector3.down * groundingDistance, Color.green);
            normal = hit.normal;
            return true;
        }
        Debug.DrawRay(transform.position + Vector3.up * groundingDistance/2f, Vector3.down * groundingDistance, Color.red);
        return false;
    }
}
