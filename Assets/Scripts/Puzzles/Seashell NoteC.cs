using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class SeashellNoteC : MonoBehaviour
{
    public AudioSource noteC;

    void OnCollisionEnter(Collision collision)
    {
        if (noteC != null)
        {
            noteC.Play();
        }
    }
}
