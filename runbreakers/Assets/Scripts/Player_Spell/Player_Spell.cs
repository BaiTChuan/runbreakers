
using UnityEngine;

public abstract class Player_Spell : MonoBehaviour
{
    [Header("Base Spell Stats")]
    [SerializeField] public float CastSpeed;
    [SerializeField] public int Damage;

    [Header("Level System")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int currentXp = 0;
    [SerializeField] private int xpToNextLevel = 100;
    [SerializeField] private float xpMultiplier = 1.5f;
    private const int maxLevel = 6;

    public abstract void Cast(Transform castPos, Vector3 direction);

    public void AddXp(int amount)
    {
        if (currentLevel >= maxLevel) return;

        currentXp += amount;
        Debug.Log(string.Format("{0} gained {1} XP. Status: LV {2} ({3}/{4})", this.name, amount, currentLevel, currentXp, xpToNextLevel));

        while (currentXp >= xpToNextLevel)
        {
            currentXp -= xpToNextLevel;
            LevelUp();
            if (currentLevel >= maxLevel)
            {
                currentXp = 0;
                break;
            }
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * xpMultiplier);
        OnLevelUp();
    }

    protected virtual void OnLevelUp()
    {
        Debug.Log(this.name + " leveled up to level " + currentLevel);
    }
}
