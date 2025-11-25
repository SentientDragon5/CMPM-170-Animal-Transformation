using UnityEngine;

public class WaterVolume : MonoBehaviour

{

    // Start is called before the first frame update
    void Start()
    {
 
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject);
        if (other.attachedRigidbody.TryGetComponent(out PlayerController player))
        {
            player.waterBodyList.Add(this);
        }

    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log(other.gameObject);
        if (other.attachedRigidbody.TryGetComponent(out PlayerController player))
        {
            player.waterBodyList.Remove(this);
        }

    }

}
