using UnityEngine;
using UnityEngine.AI;

public class enemySpawner : MonoBehaviour
{
    [Header("---- Enemy Types ----")]
    [SerializeField] GameObject basicType;
    [SerializeField] GameObject strongType;
    [SerializeField] GameObject eliteType;
    [SerializeField] GameObject mageType;

    [Header("---- Enemy Costs ----")]
    [SerializeField] int basicCost = 1;
    [SerializeField] int strongCost = 2;
    [SerializeField] int eliteCost = 3;
    [SerializeField] int mageCost = 2;

    [Header("---- Spawn Settings ----")]
    [SerializeField] float spawnRadius = 15f;
    [SerializeField] float spawnRate = 2f;
    [SerializeField] int waveBudget = 25;

    [Header("---- Spawn Chances ----")]
    [SerializeField] int basicChance = 70;
    [SerializeField] int strongChance = 20;
    [SerializeField] int eliteChance = 10;
    [SerializeField] int mageChance = 15;

    float spawnTimer;
    int currentBudget;

    void Start()
    {
        currentBudget = waveBudget;
        gamemanager.instance.updateGameGoal(currentBudget);
    }

    void Update()
    {
        if (gamemanager.instance == null || gamemanager.instance.player == null)
            return;

        if (currentBudget <= 0)
            return;

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnRate)
        {
            spawnTimer = 0f;
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
{
    GameObject enemyToSpawn = GetRandomEnemyType();

    if (enemyToSpawn == null)
        return;

    Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
    randomDirection += gamemanager.instance.player.transform.position;

    NavMeshHit hit;

    if (NavMesh.SamplePosition(randomDirection, out hit, spawnRadius, NavMesh.AllAreas))
    {
        Instantiate(enemyToSpawn, hit.position, Quaternion.identity);
        SubtractEnemyCost(enemyToSpawn);
    }
}

    GameObject GetRandomEnemyType()
    {
        bool canSpawnBasic = currentBudget >= basicCost && basicType != null;
        bool canSpawnStrong = currentBudget >= strongCost && strongType != null;
        bool canSpawnElite = currentBudget >= eliteCost && eliteType != null;
        bool canSpawnMage = currentBudget >= mageCost && mageType != null;

        int totalChance = 0;

        if (canSpawnBasic)
            totalChance += basicChance;

        if (canSpawnStrong)
            totalChance += strongChance;

        if (canSpawnElite)
            totalChance += eliteChance;

        if (canSpawnMage)
            totalChance += mageChance;

        if (totalChance == 0)
            return null;

        int roll = Random.Range(0, totalChance);

        if (canSpawnBasic)
        {
            if (roll < basicChance)
                return basicType;

            roll -= basicChance;
        }

        if (canSpawnStrong)
        {
            if (roll < strongChance)
                return strongType;

            roll -= strongChance;
        }

        if (canSpawnElite)
        {
            if (roll < eliteChance)
                return eliteType;

            roll -= eliteChance;
        }

        if (canSpawnMage)
        {
            if (roll < mageChance)
                return mageType;
        }

        return null;
    }

    void SubtractEnemyCost(GameObject spawnedEnemy)
    {
        if (spawnedEnemy == basicType)
        {
            currentBudget -= basicCost;
            gamemanager.instance.updateGameGoal(-basicCost);
        }
        else if (spawnedEnemy == strongType)
        {
            currentBudget -= strongCost;
            gamemanager.instance.updateGameGoal(-strongCost);
        }
        else if (spawnedEnemy == eliteType)
        {
            currentBudget -= eliteCost;
            gamemanager.instance.updateGameGoal(-eliteCost);
        }
        else if (spawnedEnemy == mageType)
        {
            currentBudget -= mageCost;
            gamemanager.instance.updateGameGoal(-mageCost);
        }
    }
}