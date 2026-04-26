using UnityEngine;

public abstract class Player_Spell : MonoBehaviour
{
    [SerializeField] public float CastSpeed;
    [SerializeField] public int Damage;

    [SerializeField] protected int currentLevel = 1;
    [SerializeField] private int currentXp = 0;

    [SerializeField] private int[] xpPerLevel = { 20, 30, 40, 60, 50 };
    private int maxLevel;

    public int CurrentLevel { get { return currentLevel; } }

    private void Awake()
    {
        maxLevel = xpPerLevel.Length + 1;
    }

    public abstract void Cast(Transform castPos, Vector3 direction);

    public virtual void AddXp(int amount)
    {
        if (currentLevel >= maxLevel) return;

        int xpToNextLevel = xpPerLevel[currentLevel - 1];
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
            else
            {
                xpToNextLevel = xpPerLevel[currentLevel - 1];
            }
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        Debug.Log(string.Format("{0} has leveled up to level {1}!", this.name, currentLevel));
        OnLevelUp();

        // Notify playerControl to check for potential fusions
        if (Gamemanager.instance != null && Gamemanager.instance.playerScript != null)
        {
            Gamemanager.instance.playerScript.CheckForSpellFusion();
        }
    }

    protected virtual void OnLevelUp()
    {

    }
}