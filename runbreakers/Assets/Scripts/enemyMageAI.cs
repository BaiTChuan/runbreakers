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
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform shootPoint;
    [SerializeField] float shootRate = 2f;

    [Header("---- Enemy Stats ----")]
    [SerializeField] int maxHP = 4;
    [SerializeField] int xpValue = 2;

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
            agent.stoppingDistance = stopDistance;
            agent.updateRotation = true;
            agent.updateUpAxis = true;
        }
    }

    void Update()
    {
        if (gamemanager.instance == null || gamemanager.instance.player == null || agent == null)
            return;

        shootTimer += Time.deltaTime;

        Vector3 direction = gamemanager.instance.player.transform.position - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;

        if (distance > stopDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(gamemanager.instance.player.transform.position);
        }
        else if (distance < retreatDistance)
        {
            Vector3 retreatDir = (transform.position - gamemanager.instance.player.transform.position).normalized;
            Vector3 retreatTarget = transform.position + retreatDir * retreatDistanceAmount;

            NavMeshHit hit;

            if (NavMesh.SamplePosition(retreatTarget, out hit, retreatDistanceAmount, NavMesh.AllAreas))
            {
                agent.isStopped = false;
                agent.SetDestination(hit.position);
            }

            TryShoot(direction);
        }
        else
        {
            agent.isStopped = true;
            TryShoot(direction);
        }

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void TryShoot(Vector3 direction)
    {
        if (projectilePrefab == null || shootPoint == null)
            return;

        if (shootTimer < shootRate)
            return;

        shootTimer = 0f;

        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);

        damage dmgScript = projectile.GetComponent<damage>();

        if (dmgScript != null)
        {
            dmgScript.SetDirection(direction);
        }
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
            Die();
        }
    }

    void Die()
    {
        if (gamemanager.instance == null || gamemanager.instance.player == null)
            return;

        playerControl xp = gamemanager.instance.player.GetComponent<playerControl>();

        if (xp != null)
        {
            xp.AddXP(xpValue);
        }

        Destroy(gameObject);
    }
}