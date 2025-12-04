using UnityEngine;

public class WaterVolume : MonoBehaviour
{
    BoxCollider boxCollider;
    public Vector3 GetSurfacePoint(Vector3 point)
    {
        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider>();
        return new Vector3(point.x, boxCollider.bounds.max.y, point.z);
    }
    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log(other.gameObject);
        if (other.attachedRigidbody.TryGetComponent(out PlayerController player))
        {
            player.AddWater(this);
        }

    }
    private void OnTriggerExit(Collider other)
    {
        // Debug.Log(other.gameObject);
        if (other.attachedRigidbody.TryGetComponent(out PlayerController player))
        {
            player.RemoveWater(this);
        }

    }
}
