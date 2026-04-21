using TMPro;
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
    [SerializeField] int minSpawnDistance = 8;
    [SerializeField] float startDelay = 3f;

    [Header("---- Wave Settings ----")]
    [SerializeField] int waveMax = 5;
    [SerializeField] int startingBudget = 12;
    [SerializeField] int budgetIncrease = 6;
    [SerializeField] float waveDuration = 25f;
    [SerializeField] float waveDurationIncrease = 5f;

    int waveNum;
    int enemiesAlive;
    int currentWaveBudget;
    int currentAliveBudget;

    float spawnTimer;
    float currentSpawnRate;
    float startTimer;
    float waveTimer;
    float currentWaveDuration;

    bool canSpawn;
    bool bossSpawned;
    bool isBossDefeated;
    bool waveActive;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        canSpawn = false;
        bossSpawned = false;
        isBossDefeated = false;
        waveActive = false;

        startTimer = 0f;
        spawnTimer = 0f;

        waveNum = 0;
        enemiesAlive = 0;
        currentAliveBudget = 0;

        startWave();
    }

    void Update()
    {
        if (Gamemanager.instance.bossSummoned == true)
        {
            waveNum = waveMax;
            waveTimer = 0f;
            waveActive = false;
            Gamemanager.instance.setGameGoal(enemiesAlive);
        }

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

        if (waveActive)
        {
            updateWaveTimer();
            handleWaveSpawning();
        }
        else
        {
            handleWaveEnd();
        }
    }

    void updateWaveTimer()
    {
        // This pauses the wave timer during mini-boss quest
        if (questManager.instance != null && questManager.instance.IsMiniBossQuestActive())
        {
            return;
        }

        waveTimer -= Time.deltaTime;
        if (waveTimer <= 0f)
        {
            waveTimer = 0f;
            waveActive = false;
        }
        else
        {
            Gamemanager.instance.updateWaveTimer(waveTimer);
        }
    }

    void handleWaveSpawning()
    {
        // This pauses the wave spawning during mini-boss quest
        if (questManager.instance != null && questManager.instance.IsMiniBossQuestActive())
        {
            return;
        }

        if (currentAliveBudget >= currentWaveBudget)
            return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= currentSpawnRate)
        {
            spawn();
        }
    }

    void handleWaveEnd()
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

    void startWave()
    {
        waveNum++;
        spawnTimer = 0f;
        currentAliveBudget = 0;

        currentWaveBudget = startingBudget + ((waveNum - 1) * budgetIncrease);
        currentSpawnRate = spawnRate - ((waveNum - 1) * 0.2f);
        currentWaveDuration = waveDuration + ((waveNum - 1) * waveDurationIncrease);
        waveTimer = currentWaveDuration;
        waveActive = true;

        if (currentSpawnRate < 0.6f)
        {
            currentSpawnRate = 0.6f;
        }

        if (Gamemanager.instance != null)
        {
            Gamemanager.instance.setGameGoal(enemiesAlive);
            Gamemanager.instance.setWaveCount(waveNum, waveMax);
            Gamemanager.instance.showWaveTransition(waveNum);
            Gamemanager.instance.updateWaveTimer(waveTimer);
        }

        Debug.Log("Starting Wave " + waveNum);
    }

    void spawn()
    {
        GameObject objectToSpawn = getEnemyType();

        if (objectToSpawn == null)
            return;

        int enemyCost = getEnemyCost(objectToSpawn);

        if (currentAliveBudget + enemyCost > currentWaveBudget)
            return;

        spawnTimer = 0f;

        if (trySpawnEnemy(objectToSpawn, transform.position))
        {
            enemiesAlive++;
            currentAliveBudget += enemyCost;

            if (Gamemanager.instance != null)
            {
                Gamemanager.instance.setGameGoal(enemiesAlive);
            }
        }
    }

    bool trySpawnEnemy(GameObject objectToSpawn, Vector3 centerPos)
    {
        Vector3 ranPos = Vector3.zero;

        for (int i = 0; i < 10; i++)
        {
            Vector3 randomDir = Random.insideUnitSphere;
            randomDir.y = 0f;
            randomDir.Normalize();

            float randomDistance = Random.Range(minSpawnDistance, spawnDistance);
            ranPos = centerPos + (randomDir * randomDistance);

            NavMeshHit hit;

            if (NavMesh.SamplePosition(ranPos, out hit, 2f, NavMesh.AllAreas))
            {
                float distanceFromCenter = Vector3.Distance(hit.position, centerPos);

                if (distanceFromCenter >= minSpawnDistance)
                {
                    Instantiate(objectToSpawn, hit.position, Quaternion.Euler(0, Random.Range(0, 360), 0));
                    return true;
                }
            }
        }

        return false;
    }

    GameObject getEnemyType()
    {
        bool canSpawnBasic = basicType != null && currentAliveBudget + basicCost <= currentWaveBudget;
        bool canSpawnStrong = strongType != null && currentAliveBudget + strongCost <= currentWaveBudget;
        bool canSpawnElite = eliteType != null && currentAliveBudget + eliteCost <= currentWaveBudget;
        bool canSpawnMage = mageType != null && currentAliveBudget + mageCost <= currentWaveBudget;

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

    GameObject getBossAddType()
    {
        int roll = Random.Range(0, 100);

        if (roll < 40 && basicType != null)
            return basicType;

        if (roll < 70 && strongType != null)
            return strongType;

        if (roll < 90 && mageType != null)
            return mageType;

        if (eliteType != null)
            return eliteType;

        if (basicType != null)
            return basicType;

        if (strongType != null)
            return strongType;

        if (mageType != null)
            return mageType;

        return null;
    }

    public void spawnBossAdds(int spawnTotal, Vector3 centerPos)
    {
        for (int i = 0; i < spawnTotal; i++)
        {
            GameObject addToSpawn = getBossAddType();

            if (addToSpawn != null)
            {
                trySpawnEnemy(addToSpawn, centerPos);
            }
        }
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

        currentAliveBudget -= goalValue;

        if (currentAliveBudget < 0)
        {
            currentAliveBudget = 0;
        }

        if (Gamemanager.instance != null)
        {
            Gamemanager.instance.setGameGoal(enemiesAlive);
        }
    }

    public void setBossDefeated()
    {
        isBossDefeated = true;
    }
}