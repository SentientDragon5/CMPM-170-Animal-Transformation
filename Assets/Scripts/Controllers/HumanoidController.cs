using UnityEngine;

public class HumanoidController : GenericMoveController
{    
    [Header("Walking")]
    public float moveSpeed = 5;

    [Header("Jumping")]
    public float jumpSpeed = 20;
    public float jumpMinTime = 0.2f;

    [Header("Falling")]
    public float airSpeed = 5;
    public float airControl = 0.1f;

    [Header("Grounding")]
    public LayerMask enviromentLayer = 128; // layer 7 so 2 ^ 7
    public float groundingDistance = 0.1f;

    
    float lastJumpTime;

    public override void Move(Vector3 moveInput)
    {
        // called on update, so we use this to move the player
        Vector3 groundMovement = new Vector3(moveInput.x, 0, moveInput.z);
        
        if (CheckGrounded(out Vector3 normal))
        {
            Vector3 move = groundMovement * moveSpeed;
            move.y = rb.linearVelocity.y;
            rb.linearVelocity = Vector3.ProjectOnPlane(move, normal);
        }
        else
        {
            Vector3 move = groundMovement * airSpeed;
            move.y = rb.linearVelocity.y;
            rb.linearVelocity += Physics.gravity * Time.deltaTime;
            rb.linearVelocity = Vector3.MoveTowards(rb.linearVelocity, move, airControl * moveInput.magnitude);
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
