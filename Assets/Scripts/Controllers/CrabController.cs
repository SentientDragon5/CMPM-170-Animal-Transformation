using System.Collections.Generic;
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

    [Header("Animation")]
    public float animSmoothing = 0.1f;
    
    float turnAmount;
    float lastJumpTime;


    Animator anim;
    public override void Awake()
    {
        base.Awake();
        anim = GetComponent<Animator>();
    }

    public override void Move(Vector3 moveInput)
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.z);
        if (move.magnitude > 1f) move.Normalize();
        move = rb.transform.InverseTransformDirection(move);
        bool grounded = CheckGrounded(out Vector3 normal);
        move = Vector3.ProjectOnPlane(move, normal);
        bool isMoving = moveInput.magnitude > 0.1f;
        move = new Vector3(move.x, 0, 0);
        move = rb.transform.TransformVector(move);

        if (grounded)
        {
            if (isMoving)
            {
                Transform camTransform = Camera.main.transform;
                Vector3 cameraForward = camTransform.forward;
                cameraForward.y = 0;
                cameraForward.Normalize();

                if (cameraForward != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(cameraForward, Vector3.up);
                    rb.MoveRotation(targetRotation);
                }
                rb.linearVelocity = new Vector3(move.x * moveSpeed, rb.linearVelocity.y, move.z * moveSpeed);
            }
            else
            {
                Transform camTransform = Camera.main.transform;
                Vector3 lookDir = camTransform.forward;
                lookDir.y = 0;
                lookDir = lookDir.normalized;

                if (lookDir != Vector3.zero)
                {
                    rb.MoveRotation(Quaternion.LookRotation(lookDir, Vector3.up));
                }

                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            }
        }
        else
        {
            // Airborne movement
            move.y = rb.linearVelocity.y;
            rb.linearVelocity += gravityScale * Physics.gravity * Time.deltaTime;
            rb.linearVelocity = Vector3.MoveTowards(rb.linearVelocity, move * airSpeed, airControl * moveInput.magnitude);
            rb.MoveRotation(CorrectUpRotation() * Quaternion.AngleAxis(turnAmount * airTurnSpeed * Time.deltaTime, Vector3.up));
        }
        
        UpdateAnimator();
    }

    public override void JumpAction()
    {
        if (CheckGrounded(out Vector3 normal))
        {
            lastJumpTime = Time.time;
            rb.linearVelocity += normal * jumpSpeed;
        }
    }

    public float groundingRadius = 0.1f;
    public List<Transform> groundingOrigins;
    protected bool CheckGrounded(out Vector3 normal)
    {
        normal = Vector3.zero;
        foreach (Transform t in groundingOrigins)
        {
            if (Time.time - lastJumpTime < jumpMinTime)
            {
                Debug.DrawRay(t.position, Vector3.down * 0.2f, Color.white);
                return false;
            }

            if (Physics.SphereCast(t.position, groundingRadius, Vector3.down, out RaycastHit hit, 0.2f, enviromentLayer))
            {
                Debug.DrawRay(transform.position + Vector3.up * 0.1f, Vector3.down * 0.2f, Color.green);
                normal = hit.normal;
                return true;
            }
            Debug.DrawRay(t.position, Vector3.down * 0.2f, Color.red);
        }
        return false;
    }

    
    void UpdateAnimator()
    {
        float speed = rb.linearVelocity.magnitude / moveSpeed;
        float current_speed = Mathf.Lerp(anim.GetFloat("Speed"), speed, animSmoothing * Time.deltaTime);
        anim.SetFloat("Speed", current_speed);
        var grounded = CheckGrounded(out Vector3 normal);
        anim.SetBool("Grounded", grounded);
        if (!movingAudio.isPlaying && grounded && speed > 0.1)
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
