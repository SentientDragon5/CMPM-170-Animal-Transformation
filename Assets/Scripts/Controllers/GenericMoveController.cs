using UnityEngine;

public class GenericMoveController : MonoBehaviour
{
    protected Rigidbody rb;
    void Awake()
    {
        rb = transform.parent.GetComponent<Rigidbody>();
    }

    public virtual void Move(Vector3 moveInput)
    {
        
    }
    public virtual void JumpAction()
    {
        
    }
    public virtual void Enter()
    {
        gameObject.SetActive(true);
    }
    public virtual void Exit()
    {
        gameObject.SetActive(false);
    }
}
