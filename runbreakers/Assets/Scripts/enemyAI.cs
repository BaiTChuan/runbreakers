using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [Header("---- AI Settings ----")]
    [SerializeField] float moveSpeed = 2f;

    [Header("---- Enemy Stats ----")]
    [SerializeField] int maxHP = 5;
    [SerializeField] int xpValue = 1;

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
            agent.isStopped = false;

            if (!agent.isOnNavMesh)
            {
                NavMeshHit hit;

                if (NavMesh.SamplePosition(transform.position, out hit, 2f, NavMesh.AllAreas))
                {
                    agent.Warp(hit.position);
                }
            }
        }
    }

    void Update()
    {
        if (gamemanager.instance == null || gamemanager.instance.player == null || agent == null)
            return;

        if (!agent.isOnNavMesh)
            return;

        agent.isStopped = false;
        agent.SetDestination(gamemanager.instance.player.transform.position);
    }

    public void takeDamage(int amount)
    {
        if (beingHitEffect != null)
        {
            beingHitEffect.Play();
        }

        currentHP -= amount + gamemanager.instance.playerScript.damageBuff;

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (gamemanager.instance == null)
            return;

        if (gamemanager.instance.player == null)
            return;

        playerControl xp = gamemanager.instance.player.GetComponent<playerControl>();

        if (xp != null)
        {
            xp.AddXP(xpValue);
        }

        Destroy(gameObject);
    }
}