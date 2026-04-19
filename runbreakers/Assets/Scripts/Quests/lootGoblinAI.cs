using UnityEngine;
using UnityEngine.AI;

public class lootGoblinAI : MonoBehaviour, IDamage
{
    [Header("---- Movement ----")]
    [SerializeField] float moveSpeed = 9f;
    [SerializeField] float boostedSpeed = 11f;
    [SerializeField] float lowHPSpeed = 11f;
    [SerializeField] float boostDuration = 1f;
    [SerializeField] float runDistance = 10f;
    [SerializeField] float panicDistance = 4f;
    [SerializeField] float wanderRadius = 4f;
    [SerializeField] float wanderUpdateRate = 1f;
    [SerializeField] float lowHPWanderUpdateRate = 0.35f;

    [Header("---- Goblin Stats ----")]
    [SerializeField] int maxHP = 12;
    [SerializeField] int xpValue = 4;
    [SerializeField] int goalValue = 1;
    [SerializeField] float lowHPPercent = 0.4f;

    [Header("---- Quest ----")]
    [SerializeField] string targetID = "LootGoblin";

    [Header("---- Gold Trail ----")]
    [SerializeField] GameObject goldPiecePrefab;
    [SerializeField] float goldDropRate = 2f;

    [Header("---- Hit Effect ----")]
    [SerializeField] ParticleSystem beingHitEffect;

    NavMeshAgent agent;
    int currentHP;

    float boostTimer;
    float wanderTimer;
    float goldDropTimer;

    bool isBoosted;
    bool isLowHP;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentHP = maxHP;

        boostTimer = 0f;
        wanderTimer = 0f;
        goldDropTimer = 0f;
        isBoosted = false;
        isLowHP = false;

        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.stoppingDistance = 0f;
            agent.updateRotation = true;
            agent.updateUpAxis = true;
        }

        if (questManager.instance != null)
        {
            questManager.instance.RegisterSpawnedQuestTarget(transform, targetID);
        }
    }

    void Update()
    {
        if (Gamemanager.instance == null || Gamemanager.instance.player == null || agent == null)
            return;

        updateLowHPState();

        Vector3 playerPosition = Gamemanager.instance.player.transform.position;
        Vector3 directionAway = transform.position - playerPosition;
        directionAway.y = 0f;

        float distanceToPlayer = directionAway.magnitude;

        handleSpeedBoost(distanceToPlayer);
        handleMovement(directionAway, distanceToPlayer);
        handleGoldDrops();
    }

    void updateLowHPState()
    {
        if (!isLowHP && currentHP <= Mathf.CeilToInt(maxHP * lowHPPercent))
        {
            isLowHP = true;
            isBoosted = false;

            if (agent != null)
            {
                agent.speed = lowHPSpeed;
            }
        }
    }

    void handleSpeedBoost(float distanceToPlayer)
    {
        if (isLowHP)
            return;

        if (distanceToPlayer <= panicDistance && !isBoosted)
        {
            isBoosted = true;
            boostTimer = boostDuration;

            if (agent != null)
            {
                agent.speed = boostedSpeed;
            }
        }

        if (isBoosted)
        {
            boostTimer -= Time.deltaTime;

            if (boostTimer <= 0f)
            {
                isBoosted = false;

                if (agent != null)
                {
                    agent.speed = moveSpeed;
                }
            }
        }
    }

    void handleMovement(Vector3 directionAway, float distanceToPlayer)
    {
        float currentWanderRate = isLowHP ? lowHPWanderUpdateRate : wanderUpdateRate;
        float currentWanderRadius = isLowHP ? wanderRadius * 1.5f : wanderRadius;

        wanderTimer += Time.deltaTime;

        if (distanceToPlayer <= runDistance)
        {
            Vector3 fleeDirection = directionAway.normalized;
            Vector3 fleeTarget = transform.position + (fleeDirection * runDistance);

            bool shouldAddWander = distanceToPlayer > panicDistance || isLowHP;

            if (shouldAddWander && wanderTimer >= currentWanderRate)
            {
                wanderTimer = 0f;

                Vector2 randomOffset2D = Random.insideUnitCircle * currentWanderRadius;
                Vector3 wanderOffset = new Vector3(randomOffset2D.x, 0f, randomOffset2D.y);
                fleeTarget += wanderOffset;
            }

            NavMeshHit hit;
            if (NavMesh.SamplePosition(fleeTarget, out hit, runDistance + currentWanderRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }
        else
        {
            if (wanderTimer >= currentWanderRate)
            {
                wanderTimer = 0f;

                Vector2 randomOffset2D = Random.insideUnitCircle * currentWanderRadius;
                Vector3 wanderTarget = transform.position + new Vector3(randomOffset2D.x, 0f, randomOffset2D.y);

                NavMeshHit hit;
                if (NavMesh.SamplePosition(wanderTarget, out hit, currentWanderRadius, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                }
            }
        }
    }

    void handleGoldDrops()
    {
        if (goldPiecePrefab == null)
            return;

        goldDropTimer += Time.deltaTime;

        if (goldDropTimer >= goldDropRate)
        {
            goldDropTimer = 0f;
            Instantiate(goldPiecePrefab, transform.position, Quaternion.identity);
        }
    }

    public void takeDamage(int amount)
    {
        if (beingHitEffect != null)
        {
            beingHitEffect.Play();
        }

        int totalDamage = amount;

        if (Gamemanager.instance != null && Gamemanager.instance.playerScript != null)
        {
            totalDamage += Gamemanager.instance.playerScript.damageBuff;
        }

        currentHP -= totalDamage;

        if (currentHP <= 0)
        {
            die();
        }
    }

    void die()
    {
        if (questManager.instance != null)
        {
            questManager.instance.UnregisterSpawnedQuestTarget(transform);
        }

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

        if (questManager.instance != null)
        {
            questManager.instance.ReportTargetDefeated(targetID);
        }

        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (questManager.instance != null)
        {
            questManager.instance.UnregisterSpawnedQuestTarget(transform);
        }
    }
}