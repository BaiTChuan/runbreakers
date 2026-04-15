using Unity.VisualScripting;
using UnityEngine;

public class bossSpawner : MonoBehaviour
{

    [SerializeField] GameObject model;

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            model.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (Gamemanager.instance.canSummonBoss == false)
        {
            model.SetActive(false);
        }
        else
        {
            model.SetActive(true);
        }

        if (other.CompareTag("Player") && Input.GetButton("SummonBoss") && Gamemanager.instance.canSummonBoss)
        {
            Gamemanager.instance.canSummonBoss = false;
            Gamemanager.instance.bossSummoned = true;
            Gamemanager.instance.destroyAllEnemies();
        }
    }
}
