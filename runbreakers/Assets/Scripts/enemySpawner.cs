using UnityEngine;

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
    [SerializeField] int mageCost = 3;

    [Header("---- Spawn Settings ----")]
    [SerializeField] float spawnRadius = 15f;
    [SerializeField] float spawnRate = 1f;
    [SerializeField] int waveBudget = 25;

    [Header("---- Spawn Chances ----")]
    [SerializeField] int basicChance = 78;
    [SerializeField] int strongChance = 20;
    [SerializeField] int eliteChance = 1;
    [SerializeField] int mageChance = 1;

    Transform player;
    float spawnTimer;
    int currentBudget;

    void Start()
    {
        currentBudget = waveBudget;

        gamemanager.instance.updateGameGoal(currentBudget);

        if (gamemanager.instance != null)
        {
            player = gamemanager.instance.player.transform;
        }
    }

    void Update()
    {
        if (player == null)
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

        // spawn enemy somewhere around the player
        Vector3 spawnPos = player.position + Random.insideUnitSphere * spawnRadius;
        spawnPos.y = 0f;

        GameObject newEnemy = Instantiate(enemyToSpawn, spawnPos, Quaternion.identity);

        Collider enemyCollider = newEnemy.GetComponent<Collider>();

        if (enemyCollider != null)
        {
            Vector3 adjustedPos = newEnemy.transform.position;
            adjustedPos.y = enemyCollider.bounds.extents.y;
            newEnemy.transform.position = adjustedPos;
        }

        SubtractEnemyCost(enemyToSpawn);
    }

    GameObject GetRandomEnemyType()
    {
        bool canSpawnBasic = currentBudget >= basicCost && basicType != null;
        bool canSpawnStrong = currentBudget >= strongCost && strongType != null;
        bool canSpawnElite = currentBudget >= eliteCost && eliteType != null;
        bool canSpawnMage = currentBudget >= mageCost && MageType != null;

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