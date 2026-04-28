using UnityEngine;

public class broodMotherAnimEvents : MonoBehaviour
{
    broodMotherAI parentAI;

    void Start()
    {
        parentAI = GetComponentInParent<broodMotherAI>();
    }

    public void enableDamageCollider()
    {
        if (parentAI != null)
            parentAI.enableDamageCollider();
    }

    public void endAttack()
    {
        if (parentAI != null)
            parentAI.endAttack();
    }
}