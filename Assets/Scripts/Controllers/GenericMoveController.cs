using UnityEngine;

public class GenericMoveController : MonoBehaviour
{
    protected Rigidbody rb;
    protected PlayerController player;
    public virtual void Awake()
    {
        rb = transform.parent.GetComponent<Rigidbody>();
        player = transform.parent.GetComponent<PlayerController>();
    }

    public virtual void Move(Vector3 moveInput)
    {
        
    }
    public virtual void JumpAction()
    {
        
    }
    
    protected Vector3 GetSurfacePoint()
    {
        if (!InWater || player.waterBodyList.Count == 0) return transform.position;

        float highestY = float.MinValue;
        Vector3 surfacePoint = Vector3.zero;
        foreach (var volume in player.waterBodyList)
        {
            Vector3 point = volume.GetSurfacePoint(transform.position);
            if (point.y > highestY)
            {
                highestY = point.y;
                surfacePoint = point;
            }
        }

        return surfacePoint;
    }
    protected bool InWater => player.waterBodyList.Count > 0;
    public virtual void Enter()
    {
        gameObject.SetActive(true);
    }
    public virtual void Exit()
    {
        gameObject.SetActive(false);
    }
}
