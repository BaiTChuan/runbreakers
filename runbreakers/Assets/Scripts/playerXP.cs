using UnityEngine;

public class playerXP : MonoBehaviour
{
    [Header("---- XP ----")]
    [SerializeField] int currentXP;
    [SerializeField] int currentLevel = 1;
    [SerializeField] int maxXP = 100;

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
        Debug.Log("Player gained " + amount + " XP. Total XP: " + currentXP);

        CheckLevelUp();
    }

    void CheckLevelUp()
    {
        while (currentXP >= maxXP)
        {
            currentXP -= maxXP; // carry over extra XP
            currentLevel++;

            Debug.Log("Player Leveled up! New Level: " + currentLevel);

            IncreaseXPThreshold();
        }
    }

    void IncreaseXPThreshold()
    {
        maxXP = Mathf.RoundToInt(maxXP * 1.2f);

        Debug.Log("Next level requires " + maxXP + " XP");
    }
}