using UnityEngine;

public class enemyAI : MonoBehaviour, IDamage
{
    [Header("---- AI Settings ----")]
    [SerializeField] float moveSpeed = 2f;

    [Header("---- Enemy Stats ----")]
    [SerializeField] int maxHP = 5;
    [SerializeField] int xpValue = 1;

    [Header("---- Hit Effect ----")]
    [SerializeField] ParticleSystem beingHitEffect;

    private Transform player;
    int currentHP;

    void Start()
    {
        player = gamemanager.instance.player.transform;

        currentHP = maxHP;
    }

    void Update()
    {
        if (player == null)
            return;
        
        MoveTowardsPlayer();
    }

    void MoveTowardsPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0;

        transform.position += direction.normalized * moveSpeed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public void takeDamage(int amount)
    {
        if (beingHitEffect != null)
        {
            beingHitEffect.Play();
        }

        currentHP -= amount + (gamemanager.instance.playerScript.damageBuff);

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
        playerControl xp = gamemanager.instance.player.GetComponent<playerControl>();

        if (xp != null)
        {
            xp.AddXP(xpValue);
        }

        // enemy destoryed
        Destroy(gameObject);
    }
}