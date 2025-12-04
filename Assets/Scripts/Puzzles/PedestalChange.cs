using UnityEngine;

public class PedestalChange : MonoBehaviour
{
    public int formType = 0;
    
    [ContextMenu("Update Active Form")]
    void UpdateActiveForm()
    {
        for(int i=0; i<transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i == formType);
        }
    }
    void Start()
    {
        UpdateActiveForm();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController pc))
        {
            pc.SwitchState(formType);
        }
    }
}
