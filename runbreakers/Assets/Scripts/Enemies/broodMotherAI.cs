using UnityEngine;
using UnityEngine.AI;

public class broodMotherAI : MonoBehaviour, IDamage
{
    [Header("---- AI Settings ----")]
    [SerializeField] float moveSpeed = 1.5f;
    [SerializeField] Vector3 shortestDist = Vector3.zero;

    [Header("---- Enemy Stats ----")]
    [SerializeField] int maxHP = 12;
    [SerializeField] int xpValue = 3;
    [SerializeField] int goalValue = 3;
    [SerializeField] float armorPercent = 0.2f;

    [Header("---- Summon Settings ----")]
    [SerializeField] GameObject broodMinionPrefab;
    [SerializeField] Transform spawnPoint;
    [SerializeField] float spawnRate = 4f;
    [SerializeField] int minionsPerSpawn = 1;

    [Header("---- Hit Effect ----")]
    [SerializeField] ParticleSystem beingHitEffect;

    NavMeshAgent agent;
    int currentHP;
    float spawnTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentHP = maxHP;
        spawnTimer = 0f;

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

        agent.SetDestination(Gamemanager.instance.player.transform.position - shortestDist);

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnRate)
        {
            spawnTimer = 0f;
            spawnBroodMinions();
        }
    }

    void spawnBroodMinions()
    {
        if (broodMinionPrefab == null)
            return;

        Vector3 baseSpawnPosition;

        if (spawnPoint != null)
        {
            baseSpawnPosition = spawnPoint.position;
        }
        else
        {
            baseSpawnPosition = transform.position + transform.forward;
        }

        for (int i = 0; i < minionsPerSpawn; i++)
        {
            Vector3 spawnOffset = new Vector3(Random.Range(-0.5f, 0.5f), 0f, Random.Range(-0.5f, 0.5f));
            Vector3 finalSpawnPosition = baseSpawnPosition + spawnOffset;

            GameObject minion = Instantiate(broodMinionPrefab, finalSpawnPosition, Quaternion.identity);
            ignoreSpawnCollision(minion);
        }
    }

    void ignoreSpawnCollision(GameObject spawnedMinion)
    {
        Collider[] myColliders = GetComponentsInChildren<Collider>();
        Collider[] minionColliders = spawnedMinion.GetComponentsInChildren<Collider>();

        foreach (Collider myCol in myColliders)
        {
            foreach (Collider minionCol in minionColliders)
            {
                Physics.IgnoreCollision(myCol, minionCol);
            }
        }
    }

    public void takeDamage(int amount)
    {
        if (beingHitEffect != null)
        {
            beingHitEffect.Play();
        }

        int totalDamage = amount + Gamemanager.instance.playerScript.damageBuff;
        int finalDamage = Mathf.Max(1, Mathf.RoundToInt(totalDamage * (1f - armorPercent)));

        currentHP -= finalDamage;

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