using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SeashellPuzzle : MonoBehaviour
{
    public AudioSource victorySound;
    public string solution = "abcd";
    public string current = "";
    public Door door;

    public void OnNotePlayed(string note)
    {
        if (current.Length > 0 && current[current.Length-1] == note[0])
            return;
        current += note;
        if (solution == current)
        {
            door.SetUnlocked(true);
            victorySound.Play();
        }
        else
        {
            if (!CompareValues())
            {
                current = "";
                Debug.Log("Wrong");
            }
        }
        
        for (int i = 0; i < onDoor.Count; i++)
        {
            onDoor[i].SetOn( i < current.Length);
            inWorld[i].SetOn(i < current.Length);
        }
    }

    bool CompareValues()
    {
        for (int i = 0; i < current.Length; i++)
        {
            if (solution[i] != current[i])
                return false;
        }
        return true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (currentRoutine == null)
            currentRoutine = StartCoroutine(PlaySolution());
    }

    Coroutine currentRoutine;
    
    public List<SeashellNote> inWorld;
    public List<SeashellNote> onDoor;
    public float delay = 1;

    IEnumerator PlaySolution()
    {
        foreach(var s in onDoor)
        {
            s.SetOn(true);
            s.PlayNote();
            yield return new WaitForSeconds(delay);
            s.SetOn(false);
        }
        currentRoutine = null;
        for (int i = 0; i < onDoor.Count; i++)
        {
            onDoor[i].SetOn( i < current.Length);
            inWorld[i].SetOn(i < current.Length);
        }
    }
}
