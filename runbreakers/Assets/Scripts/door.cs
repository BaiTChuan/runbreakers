using UnityEngine;

public class door : MonoBehaviour
{
    [SerializeField] GameObject model;
    [SerializeField] bool lockedByDefault;
    bool bossAlive;
    bool isLocked;

    private void Start()
    {
        bossAlive = false;
        isLocked = lockedByDefault;
        if (lockedByDefault && model != null)
            model.SetActive(true);
    }

    private void Update()
    {
        if (bossAlive)
            model.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && bossAlive == false && !isLocked)
        {
            if (model != null)
                model.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (model != null)
                model.SetActive(true);
        }
    }

    public void LockDoor()
    {
        isLocked = true;
        if (model != null)
            model.SetActive(true);
    }

    public void UnlockDoor()
    {
        isLocked = false;
    }

    public void SetBossAlive(bool alive)
    {
        bossAlive = alive;
    }
}