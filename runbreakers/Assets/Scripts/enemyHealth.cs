using UnityEngine;

public class enemyHealth : MonoBehaviour, IDamage
{
    [Header("---- Enemy Stats ----")]
    [SerializeField] int maxHP = 5;
    [SerializeField] int xpValue = 1;

    int currentHP;

    void Start()
    {
        currentHP = maxHP;
    }

    public void takeDamage(int amount)
    {
        currentHP -= amount;

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (gamemanager.instance == null)
        {
            return;
        }

        if (gamemanager.instance.player == null)
        {
            return;
        }

        //Give XP to player
        playerXP xp = gamemanager.instance.player.GetComponent<playerXP>();

        if (xp != null)
        {
            xp.AddXP(xpValue);
        }

        // enemy destoryed
        Destroy(gameObject);
    }
}