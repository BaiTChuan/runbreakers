using UnityEngine;
using UnityEngine.AI;

public class enemyMageAI : MonoBehaviour, IDamage
{
    [Header("---- Movement ----")]
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float stopDistance = 11f;
    [SerializeField] float retreatDistance = 6f;
    [SerializeField] float retreatDistanceAmount = 4f;

    [Header("---- Attack ----")]
    [SerializeField] Spell spellToCast;
    [SerializeField] Transform shootPoint;
    [SerializeField] float shootRate = 2f;

    [Header("---- Enemy Stats ----")]
    [SerializeField] int maxHP = 4;
    [SerializeField] int xpValue = 2;
    [SerializeField] int goalValue = 2;

    [Header("---- Drops ----")]
    [SerializeField] GameObject spellXPDropPrefab;

    [Header("---- Hit Effect ----")]
    [SerializeField] ParticleSystem beingHitEffect;

    int currentHP;
    float shootTimer;
    NavMeshAgent agent;
    Animator anim;
    bool isDead;

    void Start()
    {
        currentHP = maxHP;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        isDead = false;

        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.stoppingDistance = 0f;
            agent.updateRotation = false;
            agent.updateUpAxis = true;
        }
    }

    void Update()
    {
        if (isDead) return;
        if (Gamemanager.instance == null || Gamemanager.instance.player == null || agent == null)
            return;

        shootTimer += Time.deltaTime;

        Vector3 direction = Gamemanager.instance.player.transform.position - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;

        if (distance > stopDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(Gamemanager.instance.player.transform.position);
            if (anim != null) anim.SetBool("IsMoving", true);
        }
        else if (distance < retreatDistance)
        {
            Vector3 retreatDir = (transform.position - Gamemanager.instance.player.transform.position).normalized;
            Vector3 retreatTarget = transform.position + retreatDir * retreatDistanceAmount;

            NavMeshHit hit;

            if (NavMesh.SamplePosition(retreatTarget, out hit, 6f, NavMesh.AllAreas))
            {
                agent.isStopped = false;
                agent.SetDestination(hit.position);
                if (anim != null) anim.SetBool("IsMoving", true);
            }
            else
            {
                agent.ResetPath();
                agent.isStopped = true;
                if (anim != null) anim.SetBool("IsMoving", false);
            }

            tryShoot(direction);
        }
        else
        {
            agent.isStopped = true;
            if (anim != null) anim.SetBool("IsMoving", false);
            tryShoot(direction);
        }

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void tryShoot(Vector3 direction)
    {
        if (spellToCast == null || shootPoint == null)
            return;

        if (shootTimer < shootRate)
            return;

        shootTimer = 0f;

        if (anim != null) anim.SetTrigger("Attack");

        GameObject spellInstance = Instantiate(spellToCast.gameObject, shootPoint.position, shootPoint.rotation);
        Rigidbody rb = spellInstance.GetComponent<Rigidbody>();

        if (rb != null)
        {
            direction = direction.normalized;
            rb.linearVelocity = direction * spellToCast.spellToCast.speed;
        }

        transform.rotation = Quaternion.LookRotation(direction.normalized);
    }

    public void takeDamage(int amount)
    {
        if (isDead) return;

        if (beingHitEffect != null) beingHitEffect.Play();

        if (anim != null) anim.SetTrigger("HitReact");

        currentHP -= amount;

        if (currentHP <= 0) die();
    }

    void die()
    {
        if (isDead) return;
        isDead = true;

        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.enabled = false;
        }

        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
            col.enabled = false;

        if (anim != null)
        {
            anim.ResetTrigger("HitReact");
            anim.ResetTrigger("Attack");
            anim.SetTrigger("Death");
        }

        if (Gamemanager.instance != null && Gamemanager.instance.player != null)
        {
            // Drop spell XP pickup
            if (spellXPDropPrefab != null)
            {
                GameObject spellXPInstance = Instantiate(spellXPDropPrefab, transform.position, Quaternion.identity);
                SpellXPPickup spellXPPickup = spellXPInstance.GetComponent<SpellXPPickup>();
                if (spellXPPickup != null)
                    spellXPPickup.xpAmount = xpValue;
            }

            playerControl xp = Gamemanager.instance.player.GetComponent<playerControl>();
            if (xp != null) xp.AddXP(xpValue);
        }

        if (enemySpawner.instance != null)
            enemySpawner.instance.enemyDefeated(goalValue);

        Destroy(gameObject, 3f);
    }
}