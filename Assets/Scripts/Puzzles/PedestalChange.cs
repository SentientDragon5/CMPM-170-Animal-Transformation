using UnityEngine;

public class PedestalChange : MonoBehaviour
{
    public int formType = 0;
    public Transform animalParent;
    
    [ContextMenu("Update Active Form")]
    void UpdateActiveForm()
    {
        for(int i=0; i<animalParent.childCount; i++)
        {
            animalParent.GetChild(i).gameObject.SetActive(i == formType);
        }
    }
    void Start()
    {
        UpdateActiveForm();
    }
    void OnTriggerEnter(Collider other)
    {
            // Debug.Log(other.gameObject.name);
        if (other.attachedRigidbody.TryGetComponent(out PlayerController pc))
        {
            pc.SwitchState(formType);
        }
    }
}
