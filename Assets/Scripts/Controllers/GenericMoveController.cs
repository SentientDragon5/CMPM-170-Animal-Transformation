using System.Collections;
using System.Collections.Generic;
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
    public virtual void Enter(bool transition = false)
    {
        if (transition)
        {
            gameObject.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(TransitionIn());
        }
        else
        {
            gameObject.SetActive(true);
            foreach(Material m in dissolveMaterials)
            {
                m.SetFloat("_Dissolve", 1f);
            }
        }
    }
    public virtual void Exit(bool transition = false)
    {
        if (transition)
        {
            gameObject.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(TransitionOut());
        }
        else
            gameObject.SetActive(false);
    }

    public void FixUp()
    {
        // Self right the character if it turned on its side.
        // StartCoroutine(FixUpDelay());
    }

    IEnumerator FixUpDelay()
    {
        yield return new WaitForFixedUpdate();
        if (rb == null)
            rb = transform.parent.GetComponent<Rigidbody>();
        for (int i=0; i<100; i++)
        {
            Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(flatForward, Vector3.up);
            Quaternion smoothRot = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.fixedDeltaTime);
            rb.MoveRotation(targetRotation);
            rb.transform.rotation = targetRotation;
            rb.transform.eulerAngles = new Vector3(0,rb.transform.eulerAngles.y,0);
            Debug.Log("Up : " +  rb.transform.up);
            yield return new WaitForFixedUpdate();
        }
        
    }
    protected Quaternion CorrectUpRotation()
    {
        Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(flatForward, Vector3.up);
        return targetRotation;
    }

    [Header("Dissolve")]
    public List<Material> dissolveMaterials;
    public float dissolveTime = 0.1f;
    IEnumerator TransitionOut()
    {
        float start = Time.time;
        while(Time.time - start < dissolveTime)
        {
            float t = (Time.time - start) / dissolveTime;
            foreach(Material m in dissolveMaterials)
            {
                m.SetFloat("_Dissolve", Mathf.Lerp(1f,0f,t));
            }
            yield return new WaitForFixedUpdate();
        }
        
        foreach(Material m in dissolveMaterials)
        {
            m.SetFloat("_Dissolve", 0f);
        }
        gameObject.SetActive(false);
    }
    IEnumerator TransitionIn()
    {
        float start = Time.time;
        while(Time.time - start < dissolveTime)
        {
            float t = (Time.time - start) / dissolveTime;
            foreach(Material m in dissolveMaterials)
            {
                m.SetFloat("_Dissolve", Mathf.Lerp(0f,1f,t));
            }
            yield return new WaitForFixedUpdate();
        }
        
        foreach(Material m in dissolveMaterials)
        {
            m.SetFloat("_Dissolve", 1f);
        }
    }
}
