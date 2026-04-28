using UnityEngine;
using UnityEngine.AI;

public class enemyDeathBurstAI : MonoBehaviour, IDamage
{
    [Header("---- AI Settings ----")]
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] Vector3 shortestDist = Vector3.zero;

    [Header("---- Enemy Stats ----")]
    [SerializeField] int maxHP = 5;
    [SerializeField] int xpValue = 1;
    [SerializeField] int goalValue = 1;
    [SerializeField] float armorPercent = 0f;

    [Header("---- Drops ----")]
    [SerializeField] GameObject spellXPDropPrefab;

    [Header("---- Death Burst ----")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform shootPoint;
    [SerializeField] int deathProjectileCount = 8;

    [Header("---- Hit Effect ----")]
    [SerializeField] ParticleSystem beingHitEffect;

    NavMeshAgent agent;
    int currentHP;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentHP = maxHP;

        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.stoppingDistance = 0f;
            agent.updateRotation = true;
            agent.updateUpAxis = true;
        }
    }

    void Update()
    {
        if (Gamemanager.instance == null || Gamemanager.instance.player == null || agent == null)
            return;

        agent.SetDestination(Gamemanager.instance.player.transform.position - shortestDist);
    }

    public void takeDamage(int amount)
    {
        if (beingHitEffect != null)
        {
            beingHitEffect.Play();
        }

        int totalDamage = amount + Gamemanager.instance.playerScript.damageBuff;
        int finalDamage = Mathf.Max(1, Mathf.RoundToInt(totalDamage * (1f - armorPercent)));

        currentHP -= finalDamage;

        if (currentHP <= 0)
        {
            die();
        }
    }

    void die()
    {
        // Drop spell XP pickup
        if (spellXPDropPrefab != null)
        {
            GameObject spellXPInstance = Instantiate(spellXPDropPrefab, transform.position, Quaternion.identity);
            SpellXPPickup spellXPPickup = spellXPInstance.GetComponent<SpellXPPickup>();
            if (spellXPPickup != null)
            {
                spellXPPickup.xpAmount = xpValue;
            }
        }

        spawnDeathProjectiles();

        if (Gamemanager.instance != null && Gamemanager.instance.player != null)
        {
            playerControl xp = Gamemanager.instance.player.GetComponent<playerControl>();

            if (xp != null)
            {
                xp.AddXP(xpValue);
            }
        }

        if (enemySpawner.instance != null)
        {
            enemySpawner.instance.enemyDefeated(goalValue);
        }

        Destroy(gameObject);
    }

    void spawnDeathProjectiles()
    {
        if (projectilePrefab == null)
            return;

        Transform spawnTransform = shootPoint != null ? shootPoint : transform;

        if (deathProjectileCount <= 0)
            return;

        float angleStep = 360f / deathProjectileCount;

        for (int i = 0; i < deathProjectileCount; i++)
        {
            float angle = angleStep * i;
            Vector3 direction = Quaternion.Euler(0f, angle, 0f) * transform.forward;

            GameObject projectile = Instantiate(projectilePrefab, spawnTransform.position, Quaternion.identity);
        }
    }
}