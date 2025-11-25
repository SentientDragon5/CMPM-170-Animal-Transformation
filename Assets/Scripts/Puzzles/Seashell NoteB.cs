using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class SeashellNoteB : MonoBehaviour
{
    public AudioSource noteB;

    void OnCollisionEnter(Collision collision)
    {
        if (noteB != null)
        {
            noteB.Play();
        }
    }
}
