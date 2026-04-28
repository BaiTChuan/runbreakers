using UnityEngine;
using UnityEngine.AI;

public class broodMinionAI : MonoBehaviour, IDamage
{
    [Header("---- AI Settings ----")]
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] Vector3 shortestDist = Vector3.zero;

    [Header("---- Enemy Stats ----")]
    [SerializeField] int xpValue = 1;
    [SerializeField] int goalValue = 1;

    [Header("---- Drops ----")]
    [SerializeField] GameObject spellXPDropPrefab;

    [Header("---- Hit Effect ----")]
    [SerializeField] ParticleSystem beingHitEffect;

    NavMeshAgent agent;
    Animator anim;
    bool hasHitPlayer;
    bool isDead;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        hasHitPlayer = false;
        isDead = false;

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

        if (hasHitPlayer || isDead)
            return;

        agent.SetDestination(Gamemanager.instance.player.transform.position - shortestDist);

        if (anim != null)
            anim.SetBool("IsRunning", agent.velocity.magnitude > 0.1f);
    }

    public void takeDamage(int amount)
    {
        if (isDead) return;

        if (beingHitEffect != null)
            beingHitEffect.Play();

        die();
    }

    public void hitPlayerAndDie()
    {
        if (hasHitPlayer || isDead) return;
        hasHitPlayer = true;
        die();
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
        if (Gamemanager.instance != null && Gamemanager.instance.player != null)
        {
            playerControl xp = Gamemanager.instance.player.GetComponent<playerControl>();
            if (xp != null)
                xp.AddXP(xpValue);
        }

        if (enemySpawner.instance != null)
            enemySpawner.instance.enemyDefeated(goalValue);
    }
}
