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
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.z);

        if (move.magnitude > 1f) move.Normalize();
        move = rb.transform.InverseTransformDirection(move);
        
        bool grounded = CheckGrounded(out Vector3 normal);
        move = Vector3.ProjectOnPlane(move, normal);
        turnAmount = Mathf.Atan2(move.x, move.z);

        // go between local and world for the turn amount
        move = rb.transform.TransformVector(move);
        
        if (grounded)
        {
            rb.linearVelocity = move * moveSpeed;
            float turnSpeed = Mathf.Lerp(standingTurnSpeed, movingTurnSpeed, move.magnitude);
            rb.MoveRotation(transform.rotation * Quaternion.AngleAxis(turnAmount * turnSpeed * Time.deltaTime, Vector3.up));
        }
        else
        {
            move.y = rb.linearVelocity.y;
            rb.linearVelocity += gravityScale * Physics.gravity * Time.deltaTime;
            rb.linearVelocity = Vector3.MoveTowards(rb.linearVelocity, move * airSpeed, airControl * moveInput.magnitude);
            rb.MoveRotation(transform.rotation * Quaternion.AngleAxis(turnAmount * airTurnSpeed * Time.deltaTime, Vector3.up));
            // rb.transform.Rotate(0,,0);
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
