using UnityEngine;

public class door : MonoBehaviour
{
    [SerializeField] GameObject model;

    bool bossAlive;

    private void Start()
    {
        bossAlive = false;
    }

    private void Update()
    {
        if (bossAlive)
        {
            model.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && bossAlive == false)
        {
            model.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            model.SetActive(true);
        }
    }
}