using UnityEngine;

public class HumanoidController : GenericMoveController
{    
    public float moveSpeed = 5;
    public float jumpSpeed = 10;

    public override void Move(Vector3 moveInput)
    {
        // called on update, so we use this to move the player
        Vector3 groundMovement = new Vector3(moveInput.x, 0, moveInput.z);

        rb.linearVelocity = groundMovement * moveSpeed;
    }

    public override void JumpAction()
    {
        rb.linearVelocity += Vector3.up * jumpSpeed;
    }
}
