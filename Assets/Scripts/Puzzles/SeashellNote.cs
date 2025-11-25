using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class SeashellNoteA : MonoBehaviour
{
    public AudioSource note;
    public string noteName = "d";
    public UnityEvent<string> onNotePlayed;
    void OnCollisionEnter(Collision collision)
    {
        if (note != null)
        {
            note.Play();
        }
    }
}
