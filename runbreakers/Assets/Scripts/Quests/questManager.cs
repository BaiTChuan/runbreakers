using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class questManager : MonoBehaviour
{
    public static questManager instance;

    [Header("---- Point Based Quests ----")]
    [SerializeField] List<questPoint> pointQuests = new List<questPoint>();

    [Header("---- Spawned Target Quests ----")]
    [SerializeField] List<questData> spawnedTargetQuests = new List<questData>();

    [Header("---- UI ----")]
    [SerializeField] TMP_Text questNameText;
    [SerializeField] TMP_Text questObjectiveText;
    [SerializeField] TMP_Text questTimerText;
    [SerializeField] TMP_Text questDistanceText;
    [SerializeField] GameObject rewardPopupPanel;
    [SerializeField] TMP_Text rewardPopupText;
    [SerializeField] float rewardPopupDuration = 3f;
    Coroutine rewardPopupCoroutine;

    [Header("---- Quest Timing ----")]
    [SerializeField] float timeBetweenQuests = 10f;

    [Header("---- Difficulty Scaling ----")]
    [SerializeField] int extraEnemiesPerCompletedQuest = 1;
    [SerializeField] float targetHealthIncreasePerCompletedQuest = 0.1f;

    questPoint currentQuestPoint;
    questData currentQuest;
    Transform currentTrackedTarget;
    GameObject currentSpawnedQuestTarget;
    float currentQuestTimer;
    float nextQuestTimer;
    bool questActive;
    bool objectiveStarted;
    int completedQuestCount;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        questActive = false;
        objectiveStarted = false;
        currentQuest = null;
        currentQuestPoint = null;
        currentTrackedTarget = null;
        currentSpawnedQuestTarget = null;
        currentQuestTimer = 0f;
        nextQuestTimer = timeBetweenQuests;
        completedQuestCount = 0;

        foreach (questPoint point in pointQuests)
        {
            if (point != null)
            {
                point.DeactivateQuestPoint();
            }
        }

        if (rewardPopupPanel != null)
        {
            rewardPopupPanel.SetActive(false);
        }
        clearQuestUI();
    }

    void Update()
    {
        if (Gamemanager.instance == null || Gamemanager.instance.player == null)
            return;

        if (!questActive)
        {
            nextQuestTimer -= Time.deltaTime;
            if (nextQuestTimer <= 0f)
            {
                StartRandomQuest();
            }
            return;
        }

        currentQuestTimer -= Time.deltaTime;
        if (currentQuestTimer <= 0f)
        {
            if (currentQuest != null && currentQuest.questType == QuestType.SurvivalZone && objectiveStarted)
            {
                CompleteCurrentQuest();
            }
            else
            {
                FailCurrentQuest();
            }
            return;
        }

        updateQuestTimerUI();
        updateQuestDistanceUI();
    }

    public void StartRandomQuest()
    {
        List<questPoint> availablePointQuests = new List<questPoint>();
        List<questData> availableSpawnedQuests = new List<questData>();

        foreach (questPoint point in pointQuests)
        {
            if (isValidPointQuest(point))
            {
                availablePointQuests.Add(point);
            }
        }

        foreach (questData quest in spawnedTargetQuests)
        {
            if (isValidSpawnedQuest(quest))
            {
                availableSpawnedQuests.Add(quest);
            }
        }

        int totalAvailable = availablePointQuests.Count + availableSpawnedQuests.Count;
        if (totalAvailable <= 0)
        {
            Debug.Log("No more available valid quests.");
            return;
        }

        int randomIndex = Random.Range(0, totalAvailable);
        if (randomIndex < availablePointQuests.Count)
        {
            StartPointQuest(availablePointQuests[randomIndex]);
        }
        else
        {
            int spawnedIndex = randomIndex - availablePointQuests.Count;
            StartSpawnedQuest(availableSpawnedQuests[spawnedIndex]);
        }
    }

    bool isValidPointQuest(questPoint point)
    {
        if (point == null || point.questData == null) return false;
        if (point.questData.isCompleted) return false;
        if (string.IsNullOrWhiteSpace(point.questData.questName)) return false;
        if (point.questData.travelTimeLimit <= 0f) return false;
        if (point.questData.objectiveTimeLimit <= 0f) return false;
        if (point.questData.baseTargetHealthMultiplier <= 0f)
            point.questData.baseTargetHealthMultiplier = 1f;
        return true;
    }

    bool isValidSpawnedQuest(questData quest)
    {
        if (quest == null) return false;
        if (quest.isCompleted) return false;
        if (string.IsNullOrWhiteSpace(quest.questName)) return false;
        if (quest.objectiveTimeLimit <= 0f) return false;
        if (string.IsNullOrWhiteSpace(quest.targetID)) return false;
        if (quest.spawnedTargetPrefab == null) return false;
        if (quest.baseTargetHealthMultiplier <= 0f)
            quest.baseTargetHealthMultiplier = 1f;
        if (quest.spawnDistanceFromPlayer <= 0f)
            quest.spawnDistanceFromPlayer = 8f;
        return true;
    }

    public void StartPointQuest(questPoint point)
    {
        currentQuestPoint = point;
        currentQuest = point.questData;
        currentTrackedTarget = point.transform;
        currentSpawnedQuestTarget = null;

        applyQuestDifficultyScaling(currentQuest);

        currentQuest.currentCollectionCount = 0;
        currentQuestTimer = currentQuest.travelTimeLimit;
        questActive = true;
        objectiveStarted = false;

        point.ActivateQuestPoint();
        updateQuestUI();

        Debug.Log("Started Point Quest Travel Phase: " + currentQuest.questName);
        Debug.Log("Scaled Extra Enemy Count: " + currentQuest.scaledExtraEnemyCount);
        Debug.Log("Scaled Target HP Multiplier: " + currentQuest.scaledTargetHealthMultiplier);
    }

    public void StartSpawnedQuest(questData quest)
    {
        currentQuestPoint = null;
        currentQuest = quest;
        currentTrackedTarget = null;
        currentSpawnedQuestTarget = null;
        applyQuestDifficultyScaling(currentQuest);
        currentQuest.currentCollectionCount = 0;
        currentQuestTimer = currentQuest.objectiveTimeLimit;
        questActive = true;
        objectiveStarted = true;

        updateQuestUI();

        if (quest.questType == QuestType.CollectItems)
        {
            spawnCollectibleItemsNearPlayer();
        }
        else if (quest.questType == QuestType.DefeatMiniBoss)
        {
            spawnMiniBoss();
        }
        else
        {
            spawnQuestTargetNearPlayer();
        }
    }

    void spawnQuestTargetNearPlayer()
    {
        if (currentQuest == null || currentQuest.spawnedTargetPrefab == null) return;
        if (Gamemanager.instance == null || Gamemanager.instance.player == null) return;

        Vector3 playerPosition = Gamemanager.instance.player.transform.position;
        Vector2 randomCircle = Random.insideUnitCircle.normalized * currentQuest.spawnDistanceFromPlayer;
        Vector3 spawnGuess = playerPosition + new Vector3(randomCircle.x, 0f, randomCircle.y);

        NavMeshHit hit;
        Vector3 spawnPosition = spawnGuess;
        if (NavMesh.SamplePosition(spawnGuess, out hit, currentQuest.spawnDistanceFromPlayer + 5f, NavMesh.AllAreas))
        {
            spawnPosition = hit.position;
        }

        currentSpawnedQuestTarget = Instantiate(currentQuest.spawnedTargetPrefab, spawnPosition, Quaternion.identity);
        currentTrackedTarget = currentSpawnedQuestTarget.transform;
    }

    public void spawnCollectibleItemsNearPlayer()
    {
        if (currentQuest == null || currentQuest.spawnedTargetPrefab == null) return;
        if (Gamemanager.instance == null || Gamemanager.instance.player == null) return;

        Vector3 playerPosition = Gamemanager.instance.player.transform.position;
        int spawned = 0;
        int attempts = 0;
        while (spawned < currentQuest.requiredCollectionCount && attempts < 30)
        {
            attempts++;
            Vector2 randomCircle = Random.insideUnitCircle.normalized * currentQuest.spawnDistanceFromPlayer;
            Vector3 spawnGuess = playerPosition + new Vector3(randomCircle.x, 0.5f, randomCircle.y);
            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawnGuess, out hit, currentQuest.spawnDistanceFromPlayer + 5f, NavMesh.AllAreas))
            {
                Vector3 spawnPosition = new Vector3(hit.position.x, 0.5f, hit.position.z);
                Instantiate(currentQuest.spawnedTargetPrefab, spawnPosition, Quaternion.identity);
                spawned++;
            }
        }
    }

    void spawnMiniBoss()
    {
        if (currentQuest == null || currentQuest.spawnedTargetPrefab == null) return;
        if (Gamemanager.instance == null || Gamemanager.instance.player == null) return;

        Vector3 playerPosition = Gamemanager.instance.player.transform.position;
        Vector2 randomCircle = Random.insideUnitCircle.normalized * currentQuest.spawnDistanceFromPlayer;
        Vector3 spawnGuess = playerPosition + new Vector3(randomCircle.x, 0f, randomCircle.y);

        NavMeshHit hit;
        Vector3 spawnPosition = spawnGuess;
        if (NavMesh.SamplePosition(spawnGuess, out hit, currentQuest.spawnDistanceFromPlayer + 10f, NavMesh.AllAreas))
        {
            spawnPosition = hit.position;
        }

        currentSpawnedQuestTarget = Instantiate(currentQuest.spawnedTargetPrefab, spawnPosition, Quaternion.identity);
        currentTrackedTarget = currentSpawnedQuestTarget.transform;

        RegisterSpawnedQuestTarget(currentSpawnedQuestTarget.transform, currentQuest.targetID);
    }

    public void StartMiniBossFight(questPoint miniBossPoint)
    {
        questData miniBossQuest = null;

        foreach (questData q in spawnedTargetQuests)
        {
            if (q.questType == QuestType.DefeatMiniBoss && q.targetID == "SpiderMiniBoss")
            {
                miniBossQuest = q;
                break;
            }
        }

        if (miniBossQuest == null && miniBossPoint != null && miniBossPoint.questData != null)
        {
            miniBossQuest = miniBossPoint.questData;
        }

        if (miniBossQuest == null)
        {
            Debug.LogWarning("No DefeatMiniBoss quest data found!");
            return;
        }

        currentQuestPoint = miniBossPoint;
        currentQuest = miniBossQuest;
        currentTrackedTarget = null;
        currentSpawnedQuestTarget = null;

        applyQuestDifficultyScaling(currentQuest);
        currentQuest.currentCollectionCount = 0;
        currentQuestTimer = currentQuest.objectiveTimeLimit;
        questActive = true;
        objectiveStarted = true;

        SpawnMiniBossAtPoint(miniBossPoint.transform.position);

        updateQuestUI();

        if (miniBossPoint != null)
            miniBossPoint.DeactivateQuestPoint();
    }

    void SpawnMiniBossAtPoint(Vector3 roomCenter)
    {
        if (currentQuest == null || currentQuest.spawnedTargetPrefab == null) return;

        Vector3 spawnPos = roomCenter + new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));

        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPos, out hit, 10f, NavMesh.AllAreas))
        {
            spawnPos = hit.position;
        }

        currentSpawnedQuestTarget = Instantiate(currentQuest.spawnedTargetPrefab, spawnPos, Quaternion.identity);
        currentTrackedTarget = currentSpawnedQuestTarget.transform;

        RegisterSpawnedQuestTarget(currentSpawnedQuestTarget.transform, currentQuest.targetID);
    }

    public bool IsMiniBossQuestActive()
    {
        if (!questActive) return false;
        if (currentQuest == null) return false;
        if (!objectiveStarted) return false;
        return currentQuest.questType == QuestType.DefeatMiniBoss;
    }

    public void BeginPointQuestObjective()
    {
        if (!questActive || currentQuest == null) return;
        if (objectiveStarted) return;

        objectiveStarted = true;
        currentQuestTimer = currentQuest.objectiveTimeLimit;
        Debug.Log("Objective Started: " + currentQuest.questName);
    }

    public void CompleteCurrentQuest()
    {
        if (!questActive || currentQuest == null) return;

        currentQuest.isCompleted = true;
        currentQuest.isFailed = false;
        questActive = false;
        objectiveStarted = false;
        completedQuestCount++;

        if (currentQuestPoint != null)
        {
            currentQuestPoint.DeactivateQuestPoint();
        }

        giveQuestRewards(currentQuest);
        Debug.Log("Quest Completed: " + currentQuest.questName);
        clearCurrentQuest();
    }

    public void FailCurrentQuest()
    {
        if (!questActive || currentQuest == null) return;

        currentQuest.isFailed = true;
        questActive = false;
        objectiveStarted = false;

        if (currentQuestPoint != null)
        {
            currentQuestPoint.DeactivateQuestPoint();
        }
        if (currentSpawnedQuestTarget != null)
        {
            Destroy(currentSpawnedQuestTarget);
        }

        Debug.Log("Quest Failed: " + currentQuest.questName);
        clearCurrentQuest();
    }

    void clearCurrentQuest()
    {
        currentQuest = null;
        currentQuestPoint = null;
        currentTrackedTarget = null;
        currentSpawnedQuestTarget = null;
        nextQuestTimer = timeBetweenQuests;
        clearQuestUI();
    }

    public void ReportTargetDefeated(string defeatedTargetID)
    {
        if (!questActive || currentQuest == null) return;
        if (!objectiveStarted && currentQuestPoint != null) return;

        if (currentQuest.targetID == defeatedTargetID)
        {
            CompleteCurrentQuest();
        }
    }

    public void ReportItemCollected(string collectedTargetID)
    {
        if (!questActive || currentQuest == null) return;
        if (!objectiveStarted && currentQuestPoint != null) return;
        if (currentQuest.questType != QuestType.CollectItems) return;
        if (currentQuest.targetID != collectedTargetID) return;

        currentQuest.currentCollectionCount++;

        if (currentQuest.currentCollectionCount >= currentQuest.requiredCollectionCount)
        {
            CompleteCurrentQuest();
        }
    }

    public void ReportReachedQuestPoint(questPoint point)
    {
        if (!questActive || currentQuest == null) return;
        if (point != currentQuestPoint) return;

        if (!objectiveStarted)
        {
            BeginPointQuestObjective();
            return;
        }

        if (currentQuest.questType == QuestType.ReachPoint)
        {
            CompleteCurrentQuest();
        }
    }

    public void RegisterSpawnedQuestTarget(Transform targetTransform, string targetID)
    {
        if (!questActive || currentQuest == null) return;
        if (currentQuest.targetID != targetID) return;

        currentTrackedTarget = targetTransform;
    }

    public void UnregisterSpawnedQuestTarget(Transform targetTransform)
    {
        if (currentTrackedTarget == targetTransform)
        {
            currentTrackedTarget = null;
        }
        if (currentSpawnedQuestTarget != null && currentSpawnedQuestTarget.transform == targetTransform)
        {
            currentSpawnedQuestTarget = null;
        }
    }

    void applyQuestDifficultyScaling(questData quest)
    {
        float baseHealthMultiplier = quest.baseTargetHealthMultiplier <= 0f ? 1f : quest.baseTargetHealthMultiplier;
        quest.scaledExtraEnemyCount = quest.baseExtraEnemyCount + (completedQuestCount * extraEnemiesPerCompletedQuest);
        quest.scaledTargetHealthMultiplier = baseHealthMultiplier + (completedQuestCount * targetHealthIncreasePerCompletedQuest);
    }

    void giveQuestRewards(questData quest)
    {
        if (Gamemanager.instance != null && Gamemanager.instance.player != null)
        {
            playerControl player = Gamemanager.instance.player.GetComponent<playerControl>();
            if (player != null)
            {
                player.AddXP(quest.rewardXP);
            }
            Gamemanager.instance.AddGold(quest.rewardGold);
        }
        showRewardPopup(quest.rewardGold, quest.rewardXP);
        Debug.Log("Reward Gold: " + quest.rewardGold);
    }

    void showRewardPopup(int goldEarned, int xpEarned)
    {
        if (rewardPopupPanel == null || rewardPopupText == null) return;

        rewardPopupText.text = "Mission Completed!\nYou earned: " + goldEarned + " Gold, " + xpEarned + " XP";
        rewardPopupPanel.SetActive(true);

        if (rewardPopupCoroutine != null)
        {
            StopCoroutine(rewardPopupCoroutine);
        }
        rewardPopupCoroutine = StartCoroutine(hideRewardPopupAfterDelay());
    }

    IEnumerator hideRewardPopupAfterDelay()
    {
        yield return new WaitForSeconds(rewardPopupDuration);
        if (rewardPopupPanel != null)
        {
            rewardPopupPanel.SetActive(false);
        }
    }

    void updateQuestUI()
    {
        if (questNameText != null)
        {
            questNameText.text = currentQuest.questName;
        }
        if (questObjectiveText != null)
        {
            questObjectiveText.text = currentQuest.objectiveText;
        }
        updateQuestTimerUI();
        updateQuestDistanceUI();
    }

    void updateQuestTimerUI()
    {
        if (questTimerText == null || currentQuest == null) return;

        string phaseText = objectiveStarted ? "Quest Time: " : "Travel Time: ";
        questTimerText.text = phaseText + Mathf.CeilToInt(currentQuestTimer);
    }

    void updateQuestDistanceUI()
    {
        if (questDistanceText == null || currentTrackedTarget == null || Gamemanager.instance.player == null) return;

        if (currentQuest != null && currentQuest.questType == QuestType.CollectItems)
        {
            questDistanceText.text = "Collected: " + currentQuest.currentCollectionCount;
            return;
        }

        float distance = Vector3.Distance(
            Gamemanager.instance.player.transform.position,
            currentTrackedTarget.position
        );
        questDistanceText.text = "Distance: " + Mathf.CeilToInt(distance);
    }

    void clearQuestUI()
    {
        if (questNameText != null) questNameText.text = "";
        if (questObjectiveText != null) questObjectiveText.text = "";
        if (questTimerText != null) questTimerText.text = "";
        if (questDistanceText != null) questDistanceText.text = "";
    }

    public questData GetCurrentQuest() { return currentQuest; }
    public bool IsQuestActive() { return questActive; }
    public bool HasObjectiveStarted() { return objectiveStarted; }
    public int GetCompletedQuestCount() { return completedQuestCount; }
    public int GetScaledExtraEnemyCount()
    {
        if (currentQuest == null) return 0;
        return currentQuest.scaledExtraEnemyCount;
    }
    public float GetScaledTargetHealthMultiplier()
    {
        if (currentQuest == null) return 1f;
        return currentQuest.scaledTargetHealthMultiplier;
    }
    public bool IsCurrentQuestTarget(string targetID)
    {
        if (!questActive || currentQuest == null) return false;
        return currentQuest.targetID == targetID;
    }
}