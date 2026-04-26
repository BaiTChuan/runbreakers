using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class spiderMiniBossAI : MonoBehaviour, IDamage
{
    [Header("---- Movement ----")]
    [SerializeField] float moveSpeed = 2.2f;
    [SerializeField] float stopDistance = 2.5f;
    [SerializeField] float webAttackDistance = 12f;

    [Header("---- Boss Stats ----")]
    [SerializeField] int maxHP = 70;
    [SerializeField] int xpValue = 8;
    [SerializeField] int goalValue = 5;
    [SerializeField] float armorPercent = 0.3f;

    [Header("---- Drops ----")]
    [SerializeField] GameObject spellXPDropPrefab;

    [Header("---- Web Attack ----")]
    [SerializeField] GameObject webProjectilePrefab;
    [SerializeField] Transform shootPoint;
    [SerializeField] float webShootRate = 3f;
    [SerializeField] int webProjectileCount = 3;
    [SerializeField] float webSpreadAngle = 14f;

    [Header("---- Bite Attack ----")]
    [SerializeField] int biteDamage = 5;
    [SerializeField] float biteRange = 3f;
    [SerializeField] float biteRadius = 1.2f;
    [SerializeField] float biteRate = 2f;

    [Header("---- Poison ----")]
    [SerializeField] int poisonTickDamage = 1;
    [SerializeField] float poisonDuration = 4f;
    [SerializeField] float poisonTickRate = 1f;

    // Sweep attack disabled for initial release. no animation available.
    // Re-enable when sweep animation is added for full Steam release.
    [Header("---- Sweep Attack (Disabled) ----")]
    [SerializeField] int sweepDamage = 1;
    [SerializeField] float sweepRange = 5f;
    [SerializeField] float sweepRadius = 4f;
    [SerializeField] float sweepAngle = 160f;
    [SerializeField] float sweepRate = 1.25f;

    [Header("---- Egg Summon ----")]
    [SerializeField] GameObject broodEggPrefab;
    [SerializeField] Transform eggSpawnPoint;
    [SerializeField] float eggSpawnRate = 8f;
    [SerializeField] int baseEggsPerSpawn = 1;

    [Header("---- Hit Effect ----")]
    [SerializeField] ParticleSystem beingHitEffect;

    enum AttackType { None, Bite, Web }
    AttackType currentAttack;

    NavMeshAgent agent;
    Animator anim;
    int currentHP;
    int eggsPerSpawn;
    bool isDead;
    bool isAttacking;
    Vector3 lastWebDirection;

    float webTimer;
    float biteTimer;
    // float sweepTimer; // Disabled. no sweep animation available yet.
    float eggTimer;

    bool playerPoisoned;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        int startingHP = maxHP;
        eggsPerSpawn = baseEggsPerSpawn;

        if (questManager.instance != null && questManager.instance.IsCurrentQuestTarget("SpiderMiniBoss"))
        {
            float hpMultiplier = questManager.instance.GetScaledTargetHealthMultiplier();
            startingHP = Mathf.RoundToInt(maxHP * hpMultiplier);
        }

        currentHP = startingHP;
        isDead = false;
        isAttacking = false;
        currentAttack = AttackType.None;

        webTimer = 0f;
        biteTimer = 0f;
        // sweepTimer = 0f; // Disabled. no sweep animation available yet.
        eggTimer = 0f;
        playerPoisoned = false;

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
        if (isDead) return;
        if (isAttacking) return;
        if (Gamemanager.instance == null || Gamemanager.instance.player == null || agent == null)
            return;

        webTimer += Time.deltaTime;
        biteTimer += Time.deltaTime;
        // sweepTimer += Time.deltaTime; // Disabled. no sweep animation available yet.
        eggTimer += Time.deltaTime;

        Vector3 direction = Gamemanager.instance.player.transform.position - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;

        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);

        if (distance > stopDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(Gamemanager.instance.player.transform.position);
        }
        else
        {
            agent.isStopped = true;
        }

        if (anim != null)
            anim.SetBool("IsWalking", agent.velocity.magnitude > 0.1f);

        if (distance <= biteRange)
            tryBite();
        // Sweep attack disabled. no animation available yet.
        // else if (distance <= sweepRange)
        //     trySweep();
        else if (distance <= webAttackDistance)
            tryWebShot(direction);

        trySpawnEggs();
    }

    void tryWebShot(Vector3 direction)
    {
        if (webProjectilePrefab == null || shootPoint == null) return;
        if (webTimer < webShootRate) return;

        webTimer = 0f;
        isAttacking = true;
        currentAttack = AttackType.Web;
        lastWebDirection = direction;

        agent.ResetPath();
        agent.velocity = Vector3.zero;

        if (anim != null)
        {
            anim.SetBool("IsWalking", false);
            anim.SetTrigger("WebAttack");
        }
    }

    void spawnWebProjectile(Vector3 direction)
    {
        GameObject projectileInstance = Instantiate(webProjectilePrefab, shootPoint.position, Quaternion.LookRotation(direction));

        Collider[] myColliders = GetComponentsInChildren<Collider>(true);
        Collider[] projectileColliders = projectileInstance.GetComponentsInChildren<Collider>(true);

        foreach (Collider myCol in myColliders)
            foreach (Collider projectileCol in projectileColliders)
                if (myCol != null && projectileCol != null)
                    Physics.IgnoreCollision(myCol, projectileCol, true);

        spiderWebProjectile webScript = projectileInstance.GetComponent<spiderWebProjectile>();
        if (webScript != null)
            webScript.SetDirection(direction);
    }

    void tryBite()
    {
        if (biteTimer < biteRate) return;
        biteTimer = 0f;
        isAttacking = true;
        currentAttack = AttackType.Bite;

        agent.ResetPath();
        agent.velocity = Vector3.zero;

        if (anim != null)
        {
            anim.SetBool("IsWalking", false);
            anim.SetTrigger("Bite");
        }
    }

    // Called by Animation Event at the hit frame of the bite animation
    public void doBiteDamage()
    {
        if (currentAttack != AttackType.Bite) return;

        Vector3 biteCenter = transform.position + (transform.forward * biteRange * 0.5f);
        Collider[] hits = Physics.OverlapSphere(biteCenter, biteRadius);

        foreach (Collider hit in hits)
        {
            if (hit == null || hit.isTrigger) continue;
            if (!hit.CompareTag("Player")) continue;

            IDamage dmg = hit.GetComponentInParent<IDamage>();
            if (dmg != null)
            {
                dmg.takeDamage(biteDamage);
                if (!playerPoisoned)
                    StartCoroutine(applyPoison(dmg));
                break;
            }
        }
    }

    // Called by Animation Event at the frame the web is released
    public void doWebAttack()
    {
        if (currentAttack != AttackType.Web) return;

        Vector3 direction = lastWebDirection.normalized;
        if (direction == Vector3.zero) return;

        if (webProjectileCount <= 1)
        {
            spawnWebProjectile(direction);
            return;
        }

        float angleStep = webSpreadAngle / (webProjectileCount - 1);
        float startAngle = -webSpreadAngle / 2f;

        for (int i = 0; i < webProjectileCount; i++)
        {
            float currentAngle = startAngle + (angleStep * i);
            Vector3 shootDirection = Quaternion.Euler(0f, currentAngle, 0f) * direction;
            spawnWebProjectile(shootDirection);
        }
    }

    IEnumerator applyPoison(IDamage dmg)
    {
        playerPoisoned = true;
        float timer = 0f;

        while (timer < poisonDuration)
        {
            yield return new WaitForSeconds(poisonTickRate);
            if (dmg != null)
                dmg.takeDamage(poisonTickDamage);
            timer += poisonTickRate;
        }

        playerPoisoned = false;
    }

    // Sweep attack disabled — no animation available yet. Re-enable for Steam release.
    // void trySweep() { ... }

    void trySpawnEggs()
    {
        if (broodEggPrefab == null) return;
        if (eggTimer < eggSpawnRate) return;
        eggTimer = 0f;

        Vector3 baseSpawnPosition = eggSpawnPoint != null
            ? eggSpawnPoint.position
            : transform.position - (transform.forward * 1.5f);

        for (int i = 0; i < eggsPerSpawn; i++)
        {
            Vector3 spawnOffset = new Vector3(Random.Range(-0.75f, 0.75f), 0f, Random.Range(-0.75f, 0.75f));
            GameObject egg = Instantiate(broodEggPrefab, baseSpawnPosition + spawnOffset, Quaternion.identity);
            ignoreSpawnCollision(egg);
        }
    }

    void ignoreSpawnCollision(GameObject spawnedObject)
    {
        Collider[] myColliders = GetComponentsInChildren<Collider>();
        Collider[] spawnedColliders = spawnedObject.GetComponentsInChildren<Collider>();

        foreach (Collider myCol in myColliders)
            foreach (Collider spawnedCol in spawnedColliders)
                if (myCol != null && spawnedCol != null)
                    Physics.IgnoreCollision(myCol, spawnedCol);
    }

    public void takeDamage(int amount)
    {
        if (isDead) return;

        if (beingHitEffect != null)
            beingHitEffect.Play();

        int totalDamage = amount + Gamemanager.instance.playerScript.damageBuff;
        int finalDamage = Mathf.Max(1, Mathf.RoundToInt(totalDamage * (1f - armorPercent)));
        currentHP -= finalDamage;

        // HitReact disabled — no hit reaction animation available yet. Re-enable for Steam release.
        // if (anim != null) anim.SetTrigger("HitReact");

        if (currentHP <= 0)
            die();
    }
    public void endAttack()
    {
        isAttacking = false;
        currentAttack = AttackType.None;
    }

    void die()
    {
        if (isDead) return;
        isDead = true;

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

        if (questManager.instance != null)
            questManager.instance.ReportTargetDefeated("SpiderMiniBoss");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 biteCenter = transform.position + (transform.forward * biteRange * 0.5f);
        Gizmos.DrawWireSphere(biteCenter, biteRadius);

        // Sweep gizmos disabled — no sweep animation available yet.
        // Gizmos.color = Color.yellow;
        // Gizmos.DrawWireSphere(transform.position, sweepRadius);
        //
        // Vector3 leftBoundary = Quaternion.Euler(0f, -sweepAngle * 0.5f, 0f) * transform.forward * sweepRange;
        // Vector3 rightBoundary = Quaternion.Euler(0f, sweepAngle * 0.5f, 0f) * transform.forward * sweepRange;
        //
        // Gizmos.color = Color.cyan;
        // Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        // Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
    }
}