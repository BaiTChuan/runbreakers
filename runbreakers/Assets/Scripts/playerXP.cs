using UnityEngine;

public class playerXP : MonoBehaviour
{
    [Header("---- XP ----")]
    [SerializeField] int currentXP;
    [SerializeField] int currentLevel = 1;

    public int GetCurrentXP()
    {
        return currentXP;
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public void AddXP(int amount)
    {
        currentXP += amount;
        Debug.Log("Player gained " + amount + "XP. Total XP: " + currentXP);
    }
}