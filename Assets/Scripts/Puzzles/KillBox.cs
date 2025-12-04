using UnityEngine;

public class KillBox : MonoBehaviour
{
    public Transform startPosition;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController pc) && startPosition != null)
        {
            pc.transform.position = startPosition.position;
            pc.transform.rotation = startPosition.rotation;
        }
    }
}