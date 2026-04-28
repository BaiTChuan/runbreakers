using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class bossAI : MonoBehaviour, IDamage
{
    [Header("---- Movement ----")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float stopDistance = 15f;

    [Header("---- Boss Stats ----")]
    [SerializeField] int maxHP = 1000;
    [SerializeField] int xpValue = 10;
    [SerializeField] float armorPercent = 0.5f;

    [Header("---- Main Attack ----")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform shootPoint;
    [SerializeField] float projectileSpeed = 15f;
    [Range(0f, 1.2f)]
    [SerializeField] float leadAmount = 0.9f;
    [SerializeField] float velocitySmoothingTime = 0.15f;

    [Header("---- Stage 1 ----")]
    [SerializeField] float shootRateStage1 = 2f;
    [SerializeField] int projectileCountStage1 = 5;
    [SerializeField] float spreadAngleStage1 = 30f;

    [Header("---- Stage 2 ----")]
    [SerializeField] float shootRateStage2 = 1.5f;
    [SerializeField] int projectileCountStage2 = 7;
    [SerializeField] float spreadAngleStage2 = 40f;

    [Header("---- Stage 3 ----")]
    [SerializeField] float shootRateStage3 = 1.2f;
    [SerializeField] int projectileCountStage3 = 9;
    [SerializeField] float spreadAngleStage3 = 50f;

    [Header("---- Stage Transition ----")]
    [SerializeField] float transitionLength = 25f;
    [SerializeField] float healRate = 15f;
    [SerializeField] float transitionSpawnRate = 2f;
    [SerializeField] int stage2TransitionSpawnCount = 1;
    [SerializeField] int stage3TransitionSpawnCount = 2;
    [SerializeField] float stage2HealPercent = 0.85f;
    [SerializeField] float stage3HealPercent = 0.55f;

    [Header("---- Stage 3 Extra Attack ----")]
    [SerializeField] float novaAttackRate = 4f;
    [SerializeField] int novaProjectileCount = 12;

    [Header("---- Stage 3 Add Spawns ----")]
    [SerializeField] float stage3SpawnRate = 6f;
    [SerializeField] int stage3SpawnCount = 2;

    [Header("---- Transition Visuals ----")]
    [SerializeField] GameObject forceField;

    [Header("---- Hit Effect ----")]
    [SerializeField] ParticleSystem beingHitEffect;

    [Header("---- Boss HP Bar ----")]
    [SerializeField] GameObject bossHPBar;
    public Image bossCurrentHP;
    [SerializeField] TMP_Text bossCurrentHPText;

    [Header("---- Chest Drop ----")]
    [SerializeField] GameObject chestPrefab;

    int currentHP;
    int currentStage;
    int stage2TriggerHP;
    int stage3TriggerHP;
    bool bossHpBarActive = false;
    float shootTimer;
    float novaTimer;
    float addSpawnTimer;
    bool isInvulnerable;
    bool isTransitioning;

    NavMeshAgent agent;
    Animator anim;

    Vector3 lastPlayerPos;
    Vector3 estimatedPlayerVelocity;
    bool playerPosTrackingInitialized;

    void Start()
    {
        currentHP = maxHP;
        currentStage = 1;
        stage2TriggerHP = Mathf.RoundToInt(maxHP * 0.66f);
        stage3TriggerHP = Mathf.RoundToInt(maxHP * 0.33f);
        shootTimer = 0f;
        novaTimer = 0f;
        addSpawnTimer = 0f;
        isInvulnerable = false;
        isTransitioning = false;

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.stoppingDistance = 0f;
            agent.updateRotation = false;
            agent.updateUpAxis = true;
        }

        if (forceField != null)
            forceField.SetActive(false);

        if (Gamemanager.instance != null)
        {
            bossHPBar = Gamemanager.instance.GetBossHPBar();
            bossCurrentHP = Gamemanager.instance.GetBossCurrentHPBar();
            bossCurrentHPText = Gamemanager.instance.GetBossHPText();
            if (bossHPBar != null)
                bossHPBar.SetActive(false);
        }
    }

    void Update()
    {
        if (Gamemanager.instance == null || Gamemanager.instance.player == null || agent == null)
            return;

        if (!bossHpBarActive && bossHPBar != null)
        {
            bossHPBar.SetActive(true);
            bossHpBarActive = true;
        }

        updateBossBar();

        Transform playerT = Gamemanager.instance.player.transform;
        if (!playerPosTrackingInitialized)
        {
            lastPlayerPos = playerT.position;
            playerPosTrackingInitialized = true;
        }
        if (Time.deltaTime > 0f)
        {
            Vector3 rawVelocity = (playerT.position - lastPlayerPos) / Time.deltaTime;
            float t = 1f - Mathf.Exp(-Time.deltaTime / Mathf.Max(0.0001f, velocitySmoothingTime));
            estimatedPlayerVelocity = Vector3.Lerp(estimatedPlayerVelocity, rawVelocity, t);
        }
        lastPlayerPos = playerT.position;

        Vector3 toPlayer = playerT.position - transform.position;
        toPlayer.y = 0f;
        if (toPlayer != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(toPlayer);

        if (isTransitioning)
        {
            agent.isStopped = true;
            if (anim != null) anim.SetBool("IsWalking", false);
            return;
        }

        shootTimer += Time.deltaTime;
        novaTimer += Time.deltaTime;
        addSpawnTimer += Time.deltaTime;

        float distance = toPlayer.magnitude;

        if (distance > stopDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(playerT.position);
            if (anim != null)
                anim.SetBool("IsWalking", agent.velocity.magnitude > 0.1f);
        }
        else
        {
            agent.isStopped = true;
            if (anim != null) anim.SetBool("IsWalking", false);

            Vector3 aimDir = getPredictedPlayerPosition() - shootPoint.position;
            aimDir.y = 0f;
            tryShoot(aimDir);
        }

        if (currentStage == 3)
        {
            tryNovaAttack();
            tryStage3SpawnAdds();
        }
    }

    Vector3 getPredictedPlayerPosition()
    {
        Transform playerT = Gamemanager.instance.player.transform;
        Vector3 playerPos = playerT.position;

        Vector3 fromPos = shootPoint != null ? shootPoint.position : transform.position;
        float distance = Vector3.Distance(fromPos, playerPos);
        float timeToHit = distance / Mathf.Max(0.01f, projectileSpeed);

        Vector3 flatVel = new Vector3(estimatedPlayerVelocity.x, 0f, estimatedPlayerVelocity.z);
        return playerPos + flatVel * timeToHit * leadAmount;
    }

    void tryShoot(Vector3 direction)
    {
        if (projectilePrefab == null || shootPoint == null) return;
        if (shootTimer < getShootRate()) return;

        shootTimer = 0f;

        if (anim != null) anim.SetTrigger("Attack");

        int currentProjectileCount = getProjectileCount();
        float currentSpreadAngle = getSpreadAngle();

        if (currentProjectileCount <= 1)
        {
            spawnProjectile(direction.normalized);
            return;
        }

        float angleStep = currentSpreadAngle / (currentProjectileCount - 1);
        float startAngle = -currentSpreadAngle / 2f;

        for (int i = 0; i < currentProjectileCount; i++)
        {
            float currentAngle = startAngle + (angleStep * i);
            Vector3 shootDirection = Quaternion.Euler(0f, currentAngle, 0f) * direction.normalized;
            spawnProjectile(shootDirection);
        }
    }

    void tryNovaAttack()
    {
        if (projectilePrefab == null || shootPoint == null) return;
        if (novaTimer < novaAttackRate) return;

        novaTimer = 0f;

        if (anim != null) anim.SetTrigger("NovaAttack");

        float angleStep = 360f / novaProjectileCount;

        for (int i = 0; i < novaProjectileCount; i++)
        {
            float currentAngle = angleStep * i;
            Vector3 shootDirection = Quaternion.Euler(0f, currentAngle, 0f) * transform.forward;
            spawnProjectile(shootDirection.normalized);
        }
    }

    void tryStage3SpawnAdds()
    {
        if (enemySpawner.instance == null) return;
        if (addSpawnTimer < stage3SpawnRate) return;

        addSpawnTimer = 0f;
        enemySpawner.instance.spawnBossAdds(stage3SpawnCount, transform.position);
    }

    void spawnProjectile(Vector3 direction)
    {
        Vector3 flatDirection = new Vector3(direction.x, 0f, direction.z).normalized;
        GameObject projectileInstance = Instantiate(projectilePrefab, shootPoint.position, Quaternion.LookRotation(flatDirection));
        enemyProjectile proj = projectileInstance.GetComponent<enemyProjectile>();
        if (proj != null)
            proj.SetDirection(flatDirection);
    }

    float getShootRate()
    {
        if (currentStage == 1) return shootRateStage1;
        if (currentStage == 2) return shootRateStage2;
        return shootRateStage3;
    }

    int getProjectileCount()
    {
        if (currentStage == 1) return projectileCountStage1;
        if (currentStage == 2) return projectileCountStage2;
        return projectileCountStage3;
    }

    float getSpreadAngle()
    {
        if (currentStage == 1) return spreadAngleStage1;
        if (currentStage == 2) return spreadAngleStage2;
        return spreadAngleStage3;
    }

    public void takeDamage(int amount)
    {
        if (isInvulnerable) return;

        if (beingHitEffect != null) beingHitEffect.Play();

        int totalDamage = amount + Gamemanager.instance.playerScript.damageBuff;
        int finalDamage = Mathf.Max(1, Mathf.RoundToInt(totalDamage * (1f - armorPercent)));
        currentHP -= finalDamage;

        if (anim != null) anim.SetTrigger("HitReact");

        if (currentStage == 1 && currentHP <= stage2TriggerHP)
        {
            currentHP = stage2TriggerHP;
            StartCoroutine(stageTransition(2, Mathf.RoundToInt(maxHP * stage2HealPercent), stage2TransitionSpawnCount));
            return;
        }

        if (currentStage == 2 && currentHP <= stage3TriggerHP)
        {
            currentHP = stage3TriggerHP;
            StartCoroutine(stageTransition(3, Mathf.RoundToInt(maxHP * stage3HealPercent), stage3TransitionSpawnCount));
            return;
        }

        if (currentHP <= 0) die();
    }

    IEnumerator stageTransition(int nextStage, int healTargetHP, int transitionSpawnCount)
    {
        isTransitioning = true;
        isInvulnerable = true;

        if (forceField != null) forceField.SetActive(true);
        if (anim != null) anim.SetTrigger("Transition");

        shootTimer = 0f;
        novaTimer = 0f;
        addSpawnTimer = 0f;

        float timer = 0f;
        float spawnTimer = 0f;

        while (timer < transitionLength)
        {
            timer += Time.deltaTime;
            spawnTimer += Time.deltaTime;

            if (currentHP < healTargetHP)
            {
                currentHP += Mathf.RoundToInt(healRate * Time.deltaTime);
                if (currentHP > healTargetHP)
                    currentHP = healTargetHP;
            }

            if (spawnTimer >= transitionSpawnRate)
            {
                spawnTimer = 0f;
                if (enemySpawner.instance != null)
                    enemySpawner.instance.spawnBossAdds(transitionSpawnCount, transform.position);
            }

            yield return null;
        }

        if (forceField != null) forceField.SetActive(false);

        currentStage = nextStage;
        isInvulnerable = false;
        isTransitioning = false;
    }

    void die()
    {
        if (forceField != null) forceField.SetActive(false);
        if (bossHPBar != null) bossHPBar.SetActive(false);
        if (anim != null) anim.SetTrigger("Death");

        if (Gamemanager.instance != null && Gamemanager.instance.player != null)
        {
            playerControl xp = Gamemanager.instance.player.GetComponent<playerControl>();
            if (xp != null) xp.AddXP(xpValue);
        }

        if (enemySpawner.instance != null)
            enemySpawner.instance.setBossDefeated();

        if (chestPrefab != null)
        {
            Instantiate(chestPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        Destroy(gameObject, 3f);
    }

    void updateBossBar()
    {
        if (bossCurrentHP != null)
            bossCurrentHP.fillAmount = (float)currentHP / maxHP;

        if (bossCurrentHPText != null)
            bossCurrentHPText.SetText(currentHP.ToString("F0"));
    }
}