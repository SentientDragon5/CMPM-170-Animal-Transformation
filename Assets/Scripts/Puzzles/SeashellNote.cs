using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class SeashellNote : MonoBehaviour
{
    public AudioSource note;
    public string noteName = "d";
    public UnityEvent<string> onNotePlayed;
    public Color color;

    SpriteRenderer rend;
    Material mat;

    void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
        mat = Instantiate(rend.sharedMaterial);
        SetOn(false);
    }
    public void SetOn(bool on)
    {
        mat.color = color;
        mat.SetColor("_EmissionColor", on ? color : Color.black);
    }
    void OnTriggerEnter(Collider collider)
    {
        PlayNote();
    }
    void OnCollisionEnter(Collision collision)
    {
        PlayNote();
    }

    public void PlayNote()
    {
        if (note != null)
        {
            note.Play();
        }
        onNotePlayed.Invoke(noteName);
    }
}
