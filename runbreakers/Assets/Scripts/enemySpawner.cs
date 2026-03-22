using UnityEngine;
using UnityEngine.AI;

public class enemySpawner : MonoBehaviour
{
    public static enemySpawner instance;

    [Header("---- Enemy Types ----")]
    [SerializeField] GameObject basicType;
    [SerializeField] GameObject strongType;
    [SerializeField] GameObject eliteType;
    [SerializeField] GameObject mageType;
    [SerializeField] GameObject bossType;

    [Header("---- Enemy Costs ----")]
    [SerializeField] int basicCost = 1;
    [SerializeField] int strongCost = 2;
    [SerializeField] int eliteCost = 3;
    [SerializeField] int mageCost = 2;

    [Header("---- Spawn Settings ----")]
    [SerializeField] float spawnRate = 2f;
    [SerializeField] int spawnDistance = 15;
    [SerializeField] float startDelay = 3f;

    [Header("---- Wave Settings ----")]
    [SerializeField] int waveMax = 5;
    [SerializeField] int startingBudget = 12;
    [SerializeField] int budgetIncrease = 6;

    int spawnCount;
    int spawnAmount;
    int waveNum;
    int enemiesAlive;

    float spawnTimer;
    float currentSpawnRate;
    float startTimer;

    bool canSpawn;
    bool bossSpawned;
    bool isBossDefeated;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        startWave();
        canSpawn = false;
        startTimer = 0f;
    }

    void Update()
    {
        if (Gamemanager.instance == null || Gamemanager.instance.player == null)
            return;

        if (!canSpawn)
        {
            startTimer += Time.deltaTime;

            if (startTimer >= startDelay)
            {
                canSpawn = true;
            }

            return;
        }

        if (bossSpawned)
        {
            if (isBossDefeated)
            {
                Gamemanager.instance.showWin();
            }

            return;
        }

        spawnTimer += Time.deltaTime;

        if (spawnCount < spawnAmount && spawnTimer >= currentSpawnRate)
        {
            spawn();
        }

        if (spawnCount >= spawnAmount && enemiesAlive <= 0)
        {
            if (waveNum >= waveMax)
            {
                spawnBoss();
            }
            else
            {
                startWave();
            }
        }
    }
    void startWave()
    {
        waveNum++;
        spawnCount = 0;
        spawnTimer = 0f;
        enemiesAlive = 0;

        spawnAmount = startingBudget + ((waveNum - 1) * budgetIncrease);
        currentSpawnRate = spawnRate - ((waveNum - 1) * 0.2f);

        if (currentSpawnRate < 0.6f)
        {
            currentSpawnRate = 0.6f;
        }

        if (Gamemanager.instance != null)
        {
            Gamemanager.instance.setGameGoal(spawnAmount);
            Gamemanager.instance.setWaveCount(waveNum, waveMax);
            Gamemanager.instance.showWaveTransition(waveNum);
        }

        Debug.Log("Starting Wave " + waveNum);
    }

    void spawn()
    {
        GameObject objectToSpawn = getEnemyType();

        if (objectToSpawn == null)
            return;

        int enemyCost = getEnemyCost(objectToSpawn);

        if (spawnCount + enemyCost > spawnAmount)
            return;

        spawnTimer = 0f;
        spawnCount += enemyCost;

        Vector3 ranPos = Random.insideUnitSphere * spawnDistance;
        ranPos += transform.position;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(ranPos, out hit, spawnDistance, NavMesh.AllAreas))
        {
            Instantiate(objectToSpawn, hit.position, Quaternion.Euler(0, Random.Range(0, 360), 0));
            enemiesAlive++;
        }
    }

    GameObject getEnemyType()
    {
        bool canSpawnBasic = basicType != null && spawnCount + basicCost <= spawnAmount;
        bool canSpawnStrong = strongType != null && spawnCount + strongCost <= spawnAmount;
        bool canSpawnElite = eliteType != null && spawnCount + eliteCost <= spawnAmount;
        bool canSpawnMage = mageType != null && spawnCount + mageCost <= spawnAmount;

        int basicChanceTemp = 0;
        int strongChanceTemp = 0;
        int eliteChanceTemp = 0;
        int mageChanceTemp = 0;

        if (waveNum == 1)
        {
            if (canSpawnBasic)
                basicChanceTemp = 75;

            if (canSpawnStrong)
                strongChanceTemp = 25;
        }
        else if (waveNum == 2)
        {
            if (canSpawnBasic)
                basicChanceTemp = 50;

            if (canSpawnStrong)
                strongChanceTemp = 30;

            if (canSpawnMage)
                mageChanceTemp = 20;
        }
        else if (waveNum == 3)
        {
            if (canSpawnBasic)
                basicChanceTemp = 35;

            if (canSpawnStrong)
                strongChanceTemp = 30;

            if (canSpawnMage)
                mageChanceTemp = 20;

            if (canSpawnElite)
                eliteChanceTemp = 15;
        }
        else if (waveNum == 4)
        {
            if (canSpawnBasic)
                basicChanceTemp = 25;

            if (canSpawnStrong)
                strongChanceTemp = 30;

            if (canSpawnMage)
                mageChanceTemp = 20;

            if (canSpawnElite)
                eliteChanceTemp = 25;
        }
        else
        {
            if (canSpawnBasic)
                basicChanceTemp = 15;

            if (canSpawnStrong)
                strongChanceTemp = 30;

            if (canSpawnMage)
                mageChanceTemp = 20;

            if (canSpawnElite)
                eliteChanceTemp = 35;
        }

        int totalChance = basicChanceTemp + strongChanceTemp + eliteChanceTemp + mageChanceTemp;

        if (totalChance <= 0)
            return null;

        int roll = Random.Range(0, totalChance);

        if (basicChanceTemp > 0)
        {
            if (roll < basicChanceTemp)
                return basicType;

            roll -= basicChanceTemp;
        }

        if (strongChanceTemp > 0)
        {
            if (roll < strongChanceTemp)
                return strongType;

            roll -= strongChanceTemp;
        }

        if (mageChanceTemp > 0)
        {
            if (roll < mageChanceTemp)
                return mageType;

            roll -= mageChanceTemp;
        }

        if (eliteChanceTemp > 0)
        {
            if (roll < eliteChanceTemp)
                return eliteType;
        }

        return null;
    }

    int getEnemyCost(GameObject enemyType)
    {
        if (enemyType == basicType)
            return basicCost;

        if (enemyType == strongType)
            return strongCost;

        if (enemyType == eliteType)
            return eliteCost;

        if (enemyType == mageType)
            return mageCost;

        return 1;
    }

    void spawnBoss()
    {
        if (bossType == null)
        {
            Gamemanager.instance.showWin();
            return;
        }

        Vector3 ranPos = Random.insideUnitSphere * spawnDistance;
        ranPos += transform.position;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(ranPos, out hit, spawnDistance, NavMesh.AllAreas))
        {
            Instantiate(bossType, hit.position, Quaternion.identity);
            bossSpawned = true;

            if (Gamemanager.instance != null)
            {
                Gamemanager.instance.setBossText();
            }
        }
    }

    public void enemyDefeated(int goalValue)
    {
        enemiesAlive--;

        if (enemiesAlive < 0)
        {
            enemiesAlive = 0;
        }

        if (Gamemanager.instance != null)
        {
            Gamemanager.instance.updateGameGoal(-goalValue);
        }
    }

    public void setBossDefeated()
    {
        isBossDefeated = true;
    }
}