using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class SeashellNoteD : MonoBehaviour
{
    public AudioSource noteD;

    void OnCollisionEnter(Collision collision)
    {
        if (noteD != null)
        {
            noteD.Play();
        }
    }
}
