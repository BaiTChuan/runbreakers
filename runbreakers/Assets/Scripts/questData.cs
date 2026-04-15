using UnityEngine;

public enum QuestType
{
    KillTarget,
    DefeatMiniBoss,
    ReachPoint,
    SurvivalZone,
    CollectItems
}

[System.Serializable]
public class questData
{
    public int questID;
    public string questName;
    [TextArea] public string objectiveText;

    public QuestType questType;

    [Header("---- Target Info ----")]
    public string targetID;

    [Header("---- Rewards ----")]
    public int rewardXP;
    public int rewardGold;

    [Header("---- Travel Timing ----")]
    public float travelTimeLimit = 30f;

    [Header("---- Objective Timing ----")]
    public float objectiveTimeLimit = 30f;

    [Header("---- Collection Quest ----")]
    public int requiredCollectionCount = 1;

    [Header("---- Difficulty Scaling ----")]
    public int baseExtraEnemyCount = 0;
    public float baseTargetHealthMultiplier = 1f;

    [HideInInspector] public int scaledExtraEnemyCount;
    [HideInInspector] public float scaledTargetHealthMultiplier;
    [HideInInspector] public int currentCollectionCount;
    [HideInInspector] public bool isCompleted;
    [HideInInspector] public bool isFailed;
}