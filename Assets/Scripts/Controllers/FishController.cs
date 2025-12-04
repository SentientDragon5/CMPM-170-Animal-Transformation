using UnityEngine;

public class FishController : GenericMoveController
{    
    [Header("Walking")]
    public float moveSpeed = 5;
    public float movingTurnSpeed = 400;

    [Header("Jumping")]
    public float jumpSpeed = 10;
    public float jumpMinTime = 0.2f;
    public float randomSpeed = 10;

    [Header("Falling")]
    public float airSpeed = 5;
    public float airControl = 0.1f;
    public float airTurnSpeed = 200;
    public float gravityScale = 2;

    [Header("Grounding")]
    public LayerMask enviromentLayer = 128; // layer 7 so 2 ^ 7
    public float groundingDistance = 0.3f;


    [Header("Watering")]
    public LayerMask waterLayer = 16; // layer 4 so 2 ^ 4    
    public float surfaceOffset = -0.5f;

    [Header("Animation")]
    public float animSmoothing = 0.1f;

    float lastJumpTime;

    Vector3 move;
    Transform cam => Camera.main.transform;
    Animator anim;
    public override void Awake()
    {
        base.Awake();
        anim = GetComponent<Animator>();
    }
    public override void Move(Vector3 moveInput)
    {
        // Vector3 move = cam.forward * moveInput.z + cam.right * moveInput.x + Vector3.up * moveInput.y;
        
        move = new Vector3(moveInput.x, 0, moveInput.z);
        if (move.magnitude > 1f) move.Normalize();
        move = rb.transform.InverseTransformDirection(move);
        move = rb.transform.TransformVector(move);

        if(!InWater) 
            FixUp();

        if (InWater)
        {
            Quaternion lookRot = Quaternion.LookRotation(cam.forward, Vector3.up);
            lookRot = Quaternion.RotateTowards(rb.rotation, lookRot, movingTurnSpeed * Time.deltaTime);
            rb.MoveRotation(lookRot);
            rb.linearVelocity = moveInput * moveSpeed;
        }
        else if (CheckGrounded(out Vector3 normal))
        {
            // bounce off ground in a random direction
            JumpAction();
        }
        else
        {
            // fall as normal
            AirborneMovment(moveInput);
        }

        UpdateAnimator();
    }
    void AirborneMovment(Vector3 moveInput)
    {
        // add gravity to our velocity
        // slowly air strafe in a controlled way
        move = Vector3.MoveTowards(rb.linearVelocity, move * airSpeed, airControl * moveInput.magnitude);
        move.y = rb.linearVelocity.y;
        rb.linearVelocity += gravityScale * Physics.gravity * Time.deltaTime;
        // skip rotation
        // rb.MoveRotation(transform.rotation * Quaternion.AngleAxis(turnAmount * airTurnSpeed * Time.deltaTime, Vector3.up));
    }
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("hit " + InWater);
        if (InWater)
            return;
        foreach(var c in collision.contacts)
        {
            if (c.point.y < transform.position.y)
            {
                JumpAction();
            }
        }
    }

    public override void JumpAction()
    {
        if (CheckGrounded(out Vector3 normal) && (Time.time - lastJumpTime > jumpMinTime))
        {
            lastJumpTime = Time.time;
            rb.linearVelocity += (normal + new Vector3(Random.Range(-1,1),0,Random.Range(-1,1)) * randomSpeed) * jumpSpeed;
        }
    }

    protected bool CheckGrounded(out Vector3 normal)
    {
        normal = Vector3.zero;
        if (Time.time - lastJumpTime < jumpMinTime)
        {
            Debug.DrawRay(transform.position + Vector3.up * groundingDistance/2f, Vector3.down * groundingDistance, Color.white);
            return false;
        }

        if (Physics.Raycast(transform.position + Vector3.up * groundingDistance/2f, Vector3.down, out RaycastHit hit, groundingDistance, enviromentLayer))
        {
            Debug.DrawRay(transform.position + Vector3.up * groundingDistance/2f, Vector3.down * groundingDistance, Color.green);
            normal = hit.normal;
            return true;
        }
        Debug.DrawRay(transform.position + Vector3.up * groundingDistance/2f, Vector3.down * groundingDistance, Color.red);
        return false;
    }
    
    void UpdateAnimator()
    {
        float speed = rb.linearVelocity.magnitude / moveSpeed;
        if (InWater)
            speed = 2;
        float current_speed = Mathf.Lerp(anim.GetFloat("Speed"), speed, animSmoothing * Time.deltaTime);
        anim.SetFloat("Speed", current_speed * 0.9f + 0.1f);
        if (!movingAudio.isPlaying && speed > 0.1)
        {
            movingAudio.volume = speed * vol;
        }
        else
        {
            movingAudio.volume = 0;
        }
    }
    public AudioSource movingAudio;
    public float vol = 0.2f;
}