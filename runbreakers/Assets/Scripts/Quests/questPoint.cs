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
        if (!startsQuestOnPlayerEnter)
            return;

        if (!other.CompareTag("Player"))
            return;

        if (questManager.instance != null)
        {
            questManager.instance.ReportReachedQuestPoint(this);
        }
    }
}