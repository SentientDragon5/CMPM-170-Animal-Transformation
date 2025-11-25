using TMPro;
using UnityEngine;

public class SeashellPuzzle : MonoBehaviour
{
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
        }
        else
        {
            if (!CompareValues())
            {
                current = "";
                Debug.Log("Wrong");
            }
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
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
