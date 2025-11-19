using UnityEngine;

public class FishController : GenericMoveController
{    
    public float moveSpeed = 5;
    public float jumpSpeed = 10;
    public bool inWater = false;
    public LayerMask waterLayer = 32;
    public LayerMask enviromentLayer = 128;
    public override void Move(Vector3 moveInput)
    {
        // called on update, so we use this to move the player
        Vector3 groundMovement = new Vector3(moveInput.x, 0, moveInput.z);
        if (CheckWater(out Vector3 wnormal))
        {
            Vector3 move = groundMovement * moveSpeed;
            rb.linearVelocity = Vector3.ProjectOnPlane(move, wnormal);
        }
        else if (CheckGrounded(out Vector3 normal))
        {

        }
        else
        {
            rb.linearVelocity += Physics.gravity;
        }
    }

    public override void JumpAction()
    {
        if (CheckGrounded(out Vector3 normal))
        {
            rb.linearVelocity += Vector3.up * jumpSpeed;
        }
    }

    protected bool CheckWater(out Vector3 normal)
    {
        normal = Vector3.zero;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 0.1f, waterLayer))
        {
            normal = hit.normal;
            return true;
        }
        return false;
    }


    protected bool CheckGrounded(out Vector3 normal)
    {
        normal = Vector3.zero;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 0.1f, enviromentLayer))
        {
            normal = hit.normal;
            return true;
        }
        return false;
    }
}
