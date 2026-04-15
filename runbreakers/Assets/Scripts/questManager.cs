using System.Collections.Generic;
using UnityEngine;
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

    [Header("---- Quest Timing ----")]
    [SerializeField] float timeBetweenQuests = 10f;

    [Header("---- Difficulty Scaling ----")]
    [SerializeField] int extraEnemiesPerCompletedQuest = 1;
    [SerializeField] float targetHealthIncreasePerCompletedQuest = 0.1f;

    questPoint currentQuestPoint;
    questData currentQuest;
    Transform currentTrackedTarget;

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
            if (point != null && point.questData != null && !point.questData.isCompleted)
            {
                availablePointQuests.Add(point);
            }
        }

        foreach (questData quest in spawnedTargetQuests)
        {
            if (quest != null && !quest.isCompleted)
            {
                availableSpawnedQuests.Add(quest);
            }
        }

        int totalAvailable = availablePointQuests.Count + availableSpawnedQuests.Count;

        if (totalAvailable <= 0)
        {
            Debug.Log("No more available quests.");
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

    void StartPointQuest(questPoint point)
    {
        currentQuestPoint = point;
        currentQuest = point.questData;
        currentTrackedTarget = point.transform;

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

    void StartSpawnedQuest(questData quest)
    {
        currentQuestPoint = null;
        currentQuest = quest;
        currentTrackedTarget = null;

        applyQuestDifficultyScaling(currentQuest);

        currentQuest.currentCollectionCount = 0;
        currentQuestTimer = currentQuest.objectiveTimeLimit;
        questActive = true;
        objectiveStarted = true;

        updateQuestUI();

        Debug.Log("Started Spawned Quest: " + currentQuest.questName);
        Debug.Log("Scaled Extra Enemy Count: " + currentQuest.scaledExtraEnemyCount);
        Debug.Log("Scaled Target HP Multiplier: " + currentQuest.scaledTargetHealthMultiplier);
    }

    public void BeginPointQuestObjective()
    {
        if (!questActive || currentQuest == null)
            return;

        if (objectiveStarted)
            return;

        objectiveStarted = true;
        currentQuestTimer = currentQuest.objectiveTimeLimit;

        Debug.Log("Objective Started: " + currentQuest.questName);
    }

    public void CompleteCurrentQuest()
    {
        if (!questActive || currentQuest == null)
            return;

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
        if (!questActive || currentQuest == null)
            return;

        currentQuest.isFailed = true;

        questActive = false;
        objectiveStarted = false;

        if (currentQuestPoint != null)
        {
            currentQuestPoint.DeactivateQuestPoint();
        }

        Debug.Log("Quest Failed: " + currentQuest.questName);

        clearCurrentQuest();
    }

    void clearCurrentQuest()
    {
        currentQuest = null;
        currentQuestPoint = null;
        currentTrackedTarget = null;
        nextQuestTimer = timeBetweenQuests;

        clearQuestUI();
    }

    public void ReportTargetDefeated(string defeatedTargetID)
    {
        if (!questActive || currentQuest == null)
            return;

        if (!objectiveStarted && currentQuestPoint != null)
            return;

        if (currentQuest.targetID == defeatedTargetID)
        {
            CompleteCurrentQuest();
        }
    }

    public void ReportItemCollected(string collectedTargetID)
    {
        if (!questActive || currentQuest == null)
            return;

        if (!objectiveStarted && currentQuestPoint != null)
            return;

        if (currentQuest.questType != QuestType.CollectItems)
            return;

        if (currentQuest.targetID != collectedTargetID)
            return;

        currentQuest.currentCollectionCount++;

        if (currentQuest.currentCollectionCount >= currentQuest.requiredCollectionCount)
        {
            CompleteCurrentQuest();
        }
    }

    public void ReportReachedQuestPoint(questPoint point)
    {
        if (!questActive || currentQuest == null)
            return;

        if (point != currentQuestPoint)
            return;

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
        if (!questActive || currentQuest == null)
            return;

        if (currentQuest.targetID != targetID)
            return;

        currentTrackedTarget = targetTransform;
    }

    public void UnregisterSpawnedQuestTarget(Transform targetTransform)
    {
        if (currentTrackedTarget == targetTransform)
        {
            currentTrackedTarget = null;
        }
    }

    void applyQuestDifficultyScaling(questData quest)
    {
        quest.scaledExtraEnemyCount = quest.baseExtraEnemyCount + (completedQuestCount * extraEnemiesPerCompletedQuest);
        quest.scaledTargetHealthMultiplier = quest.baseTargetHealthMultiplier + (completedQuestCount * targetHealthIncreasePerCompletedQuest);
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
        }

        Debug.Log("Reward Gold: " + quest.rewardGold);
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
        if (questTimerText == null || currentQuest == null)
            return;

        string phaseText = objectiveStarted ? "Quest Time: " : "Travel Time: ";
        questTimerText.text = phaseText + Mathf.CeilToInt(currentQuestTimer);
    }

    void updateQuestDistanceUI()
    {
        if (questDistanceText == null || currentTrackedTarget == null || Gamemanager.instance.player == null)
            return;

        float distance = Vector3.Distance(
            Gamemanager.instance.player.transform.position,
            currentTrackedTarget.position
        );

        questDistanceText.text = "Distance: " + Mathf.CeilToInt(distance);
    }

    void clearQuestUI()
    {
        if (questNameText != null)
        {
            questNameText.text = "";
        }

        if (questObjectiveText != null)
        {
            questObjectiveText.text = "";
        }

        if (questTimerText != null)
        {
            questTimerText.text = "";
        }

        if (questDistanceText != null)
        {
            questDistanceText.text = "";
        }
    }

    public questData GetCurrentQuest()
    {
        return currentQuest;
    }

    public bool IsQuestActive()
    {
        return questActive;
    }

    public bool HasObjectiveStarted()
    {
        return objectiveStarted;
    }

    public int GetCompletedQuestCount()
    {
        return completedQuestCount;
    }

    public int GetScaledExtraEnemyCount()
    {
        if (currentQuest == null)
            return 0;

        return currentQuest.scaledExtraEnemyCount;
    }

    public float GetScaledTargetHealthMultiplier()
    {
        if (currentQuest == null)
            return 1f;

        return currentQuest.scaledTargetHealthMultiplier;
    }

    public bool IsCurrentQuestTarget(string targetID)
    {
        if (!questActive || currentQuest == null)
            return false;

        return currentQuest.targetID == targetID;
    }
}