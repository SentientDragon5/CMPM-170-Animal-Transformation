using UnityEngine;

public class WaterVolume : MonoBehaviour
{
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
