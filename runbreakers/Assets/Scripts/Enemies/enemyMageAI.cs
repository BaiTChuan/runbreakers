using UnityEngine;
using UnityEngine.AI;

public class enemyMageAI : MonoBehaviour, IDamage
{
    [Header("---- Movement ----")]
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float stopDistance = 8f;
    [SerializeField] float retreatDistance = 4f;
    [SerializeField] float retreatDistanceAmount = 3f;

    [Header("---- Attack ----")]
    [SerializeField] Spell spellToCast;
    [SerializeField] Transform shootPoint;
    [SerializeField] float shootRate = 2f;

    [Header("---- Enemy Stats ----")]
    [SerializeField] int maxHP = 4;
    [SerializeField] int xpValue = 2;
    [SerializeField] int goalValue = 2;

    [Header("---- Hit Effect ----")]
    [SerializeField] ParticleSystem beingHitEffect;

    int currentHP;
    float shootTimer;
    NavMeshAgent agent;

    void Start()
    {
        currentHP = maxHP;
        agent = GetComponent<NavMeshAgent>();

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
            }
            else
            {
                agent.ResetPath();
                agent.isStopped = true;
            }

            tryShoot(direction);
        }
        else
        {
            agent.isStopped = true;
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
        if (beingHitEffect != null)
        {
            beingHitEffect.Play();
        }

        currentHP -= amount;

        if (currentHP <= 0)
        {
            die();
        }
    }

    void die()
    {
        if (Gamemanager.instance == null || Gamemanager.instance.player == null)
            return;

        playerControl xp = Gamemanager.instance.player.GetComponent<playerControl>();

        if (xp != null)
        {
            xp.AddXP(xpValue);
        }

        if (enemySpawner.instance != null)
        {
            enemySpawner.instance.enemyDefeated(goalValue);
        }

        Destroy(gameObject);
    }
}