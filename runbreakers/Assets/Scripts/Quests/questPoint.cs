using UnityEngine;

public class questPoint : MonoBehaviour
{
    [Header("---- Quest Data ----")]
    public questData questData;

    [Header("---- Point Settings ----")]
    [SerializeField] bool startsQuestOnPlayerEnter = true;

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void ActivateQuestPoint()
    {
        gameObject.SetActive(true);
    }

    public void DeactivateQuestPoint()
    {
        gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (questManager.instance != null)
        {
            if (gameObject.name.Contains("MiniBoss") || (questData != null && questData.questType == QuestType.DefeatMiniBoss))
            {
                questManager.instance.StartMiniBossFight(this);
                return;
            }

            if (!startsQuestOnPlayerEnter)
                return;

            questManager.instance.ReportReachedQuestPoint(this);
        }
    }
}