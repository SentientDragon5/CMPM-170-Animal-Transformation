using UnityEngine;

public class Door : MonoBehaviour
{
    public Material mat;
    void Start()
    {
        SetUnlocked(false);
    }

    public void SetUnlocked(bool unlocked)
    {
        mat.color = unlocked ? Color.green : Color.red;
    }
}
