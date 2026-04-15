using UnityEngine;

public class broodMinionContact : MonoBehaviour
{
    [SerializeField] int damageAmount = 1;

    broodMinionAI parentMinion;
    bool hasTriggered;

    void Start()
    {
        parentMinion = GetComponentInParent<broodMinionAI>();
        hasTriggered = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered)
            return;

        if (other.isTrigger)
            return;

        if (!other.CompareTag("Player"))
            return;

        IDamage dmg = other.GetComponentInParent<IDamage>();

        if (dmg != null)
        {
            dmg.takeDamage(damageAmount);
            hasTriggered = true;

            if (parentMinion != null)
            {
                parentMinion.hitPlayerAndDie();
            }
        }
    }
}
