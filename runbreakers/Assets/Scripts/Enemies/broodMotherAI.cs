using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class broodMotherAI : MonoBehaviour, IDamage
{
    [Header("---- AI Settings ----")]
    [SerializeField] float moveSpeed = 1.5f;
    [SerializeField] Vector3 shortestDist = Vector3.zero;
    [SerializeField] float attackRange = 2.5f;
    [SerializeField] float attackCooldown = 2f;

    [Header("---- Enemy Stats ----")]
    [SerializeField] int maxHP = 12;
    [SerializeField] int xpValue = 3;
    [SerializeField] int goalValue = 3;
    [SerializeField] float armorPercent = 0.2f;

    [Header("---- Drops ----")]
    [SerializeField] GameObject spellXPDropPrefab;

    [Header("---- Summon Settings ----")]
    [SerializeField] GameObject broodMinionPrefab;
    [SerializeField] Transform spawnPoint;
    [SerializeField] float spawnRate = 4f;
    [SerializeField] int minionsPerSpawn = 1;

    [Header("---- Attack ----")]
    [SerializeField] Collider damageCollider;

    [Header("---- Hit Effect ----")]
    [SerializeField] ParticleSystem beingHitEffect;

    NavMeshAgent agent;
    Animator anim;
    int currentHP;
    float spawnTimer;
    float attackTimer;
    bool isDead;
    bool isAttacking;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        currentHP = maxHP;
        spawnTimer = 0f;
        attackTimer = attackCooldown;
        isDead = false;
        isAttacking = false;

        if (damageCollider != null)
            damageCollider.enabled = false;

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

        if (isDead || isAttacking) return;

        float distToPlayer = Vector3.Distance(transform.position, Gamemanager.instance.player.transform.position);

        attackTimer += Time.deltaTime;

        if (distToPlayer <= attackRange && attackTimer >= attackCooldown)
        {
            startAttack();
            return;
        }

        agent.SetDestination(Gamemanager.instance.player.transform.position - shortestDist);

        if (anim != null)
            anim.SetBool("IsWalking", agent.velocity.magnitude > 0.1f);

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnRate)
        {
            spawnTimer = 0f;
            spawnBroodMinions();
        }
    }

    void startAttack()
    {
        isAttacking = true;
        attackTimer = 0f;

        agent.ResetPath();
        agent.velocity = Vector3.zero;

        if (anim != null)
        {
            anim.SetBool("IsWalking", false);
            anim.SetTrigger("Attack");
        }
    }

    // Called by Animation Event at the hit frame of the attack animation
    public void enableDamageCollider()
    {
        if (damageCollider != null)
            damageCollider.enabled = true;
    }

    // Called by Animation Event at the end of the attack animation
    public void endAttack()
    {
        if (damageCollider != null)
            damageCollider.enabled = false;

        isAttacking = false;
    }

    void spawnBroodMinions()
    {
        if (broodMinionPrefab == null)
            return;

        Vector3 baseSpawnPosition = spawnPoint != null
            ? spawnPoint.position
            : transform.position + transform.forward;

        for (int i = 0; i < minionsPerSpawn; i++)
        {
            Vector3 spawnOffset = new Vector3(Random.Range(-0.5f, 0.5f), 0f, Random.Range(-0.5f, 0.5f));
            GameObject minion = Instantiate(broodMinionPrefab, baseSpawnPosition + spawnOffset, Quaternion.identity);
            ignoreSpawnCollision(minion);
        }
    }

    void ignoreSpawnCollision(GameObject spawnedMinion)
    {
        Collider[] myColliders = GetComponentsInChildren<Collider>();
        Collider[] minionColliders = spawnedMinion.GetComponentsInChildren<Collider>();

        foreach (Collider myCol in myColliders)
            foreach (Collider minionCol in minionColliders)
                Physics.IgnoreCollision(myCol, minionCol);
    }

    public void takeDamage(int amount)
    {
        if (isDead) return;

        if (beingHitEffect != null)
            beingHitEffect.Play();

        int totalDamage = amount + Gamemanager.instance.playerScript.damageBuff;
        int finalDamage = Mathf.Max(1, Mathf.RoundToInt(totalDamage * (1f - armorPercent)));
        currentHP -= finalDamage;

        if (currentHP <= 0)
            die();
    }

    void die()
    {
        if (isDead) return;
        isDead = true;

        if (damageCollider != null)
            damageCollider.enabled = false;

        if (agent != null)
            agent.enabled = false;

        foreach (Collider col in GetComponentsInChildren<Collider>())
            col.enabled = false;

        if (anim != null)
            anim.SetTrigger("Death");

        dropLoot();
        giveRewards();

        Destroy(gameObject, 2f);
    }

    void dropLoot()
    {
        if (spellXPDropPrefab != null)
        {
            GameObject spellXPInstance = Instantiate(spellXPDropPrefab, transform.position, Quaternion.identity);
            SpellXPPickup spellXPPickup = spellXPInstance.GetComponent<SpellXPPickup>();
            if (spellXPPickup != null)
                spellXPPickup.xpAmount = xpValue;
        }
    }

    void giveRewards()
    {
        if (Gamemanager.instance == null || Gamemanager.instance.player == null) return;

        playerControl xp = Gamemanager.instance.player.GetComponent<playerControl>();
        if (xp != null)
            xp.AddXP(xpValue);

        if (enemySpawner.instance != null)
            enemySpawner.instance.enemyDefeated(goalValue);
    }
}