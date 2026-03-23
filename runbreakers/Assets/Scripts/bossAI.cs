using UnityEngine;
using UnityEngine.AI;

public class bossAI : MonoBehaviour, IDamage
{
    [Header("---- Movement ----")]
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float stopDistance = 10f;

    [Header("---- Boss Stats ----")]
    [SerializeField] int maxHP = 100;
    [SerializeField] int xpValue = 10;

    [Header("---- Attack ----")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform shootPoint;
    [SerializeField] float shootRate = 2f;
    [SerializeField] int projectileCount = 5;
    [SerializeField] float spreadAngle = 30f;

    [Header("---- Hit Effect ----")]
    [SerializeField] ParticleSystem beingHitEffect;

    int currentHP;
    float shootTimer;
    NavMeshAgent agent;

    void Start()
    {
        currentHP = maxHP;
        shootTimer = 0f;
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
        if (projectilePrefab == null || shootPoint == null)
            return;

        if (shootTimer < shootRate)
            return;

        shootTimer = 0f;

        if (projectileCount <= 1)
        {
            GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
            damage dmgScript = projectile.GetComponent<damage>();

            if (dmgScript != null)
            {
                dmgScript.SetDirection(direction);
            }

            return;
        }

        float angleStep = spreadAngle / (projectileCount - 1);
        float startAngle = -spreadAngle / 2f;

        for (int i = 0; i < projectileCount; i++)
        {
            float currentAngle = startAngle + (angleStep * i);
            Vector3 shootDirection = Quaternion.Euler(0f, currentAngle, 0f) * direction.normalized;

            GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
            damage dmgScript = projectile.GetComponent<damage>();

            if (dmgScript != null)
            {
                dmgScript.SetDirection(shootDirection);
            }
        }
    }

    public void takeDamage(int amount)
    {
        if (beingHitEffect != null)
        {
            beingHitEffect.Play();
        }

        currentHP -= amount + Gamemanager.instance.playerScript.damageBuff;

        if (currentHP <= 0)
        {
            die();
        }
    }

    void die()
    {
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
            enemySpawner.instance.setBossDefeated();
        }

        Destroy(gameObject);
    }
}