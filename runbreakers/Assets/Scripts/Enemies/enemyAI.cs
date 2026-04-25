using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [Header("---- AI Settings ----")]
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] Vector3 shortestDist = Vector3.zero;

    [Header("---- Enemy Stats ----")]
    [SerializeField] int maxHP = 5;
    [SerializeField] int xpValue = 1;
    [SerializeField] int goalValue = 1;
    [SerializeField] float armorPercent = 0f;

    [Header("---- Attack ----")]
    [SerializeField] float attackRange = 1.5f;
    [SerializeField] float attackRate = 1.5f;
    [SerializeField] int attackDamage = 1;

    [Header("---- Hit Effect ----")]
    [SerializeField] ParticleSystem beingHitEffect;

    NavMeshAgent agent;
    Animator anim;
    int currentHP;
    float attackTimer;
    bool isDead;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        currentHP = maxHP;
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
        if (isDead) return;
        if (Gamemanager.instance == null || Gamemanager.instance.player == null || agent == null)
            return;

        attackTimer += Time.deltaTime;
        float distance = Vector3.Distance(transform.position, Gamemanager.instance.player.transform.position);

        if (distance > attackRange)
        {
            agent.SetDestination(Gamemanager.instance.player.transform.position - shortestDist);
        }
        else
        {
            agent.ResetPath();
            tryAttack();
        }
    }

    void tryAttack()
    {
        if (attackTimer < attackRate) return;
        attackTimer = 0f;

        if (anim != null) anim.SetTrigger("Attack");

        IDamage target = Gamemanager.instance.player.GetComponent<IDamage>();
        if (target != null) target.takeDamage(attackDamage);
    }

    public void takeDamage(int amount)
    {
        if (isDead) return;
        if (beingHitEffect != null) beingHitEffect.Play();

        if (anim != null) anim.SetTrigger("HitReact");

        int totalDamage = amount + Gamemanager.instance.playerScript.damageBuff;
        int finalDamage = Mathf.Max(1, Mathf.RoundToInt(totalDamage * (1f - armorPercent)));
        currentHP -= finalDamage;

        if (currentHP <= 0) die();
    }

    void die()
    {
        isDead = true;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.enabled = false;

        if (anim != null)
        {
            anim.ResetTrigger("HitReact");
            anim.ResetTrigger("Attack");
            anim.SetTrigger("Death");
        }

        if (Gamemanager.instance != null && Gamemanager.instance.player != null)
        {
            playerControl xp = Gamemanager.instance.player.GetComponent<playerControl>();
            if (xp != null) xp.AddXP(xpValue);
        }

        if (enemySpawner.instance != null)
            enemySpawner.instance.enemyDefeated(goalValue);

        Destroy(gameObject, 3f);
    }
}