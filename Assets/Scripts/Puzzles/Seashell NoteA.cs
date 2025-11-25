using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class SeashellNoteA : MonoBehaviour
{
    public AudioSource noteA;

    void OnCollisionEnter(Collision collision)
    {
        if (noteA != null)
        {
            noteA.Play();
        }
    }
}
