using UnityEngine;

public class RingOfFire : MonoBehaviour
{
   
   bool playerInside = false;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = true;

        if (questManager.instance != null)
            questManager.instance.BeginPointQuestObjective();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;

        if (!playerInside && questManager.instance != null && questManager.instance.IsQuestActive())
            questManager.instance.FailCurrentQuest();
        
    }
}
