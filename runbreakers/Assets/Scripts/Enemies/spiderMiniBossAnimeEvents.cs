using UnityEngine;

public class spiderMiniBossAnimEvents : MonoBehaviour
{
    spiderMiniBossAI parentAI;

    void Start()
    {
        parentAI = GetComponentInParent<spiderMiniBossAI>();
    }

    // Called by Animation Event at the hit frame of the bite animation
    public void doBiteDamage()
    {
        if (parentAI != null)
            parentAI.doBiteDamage();
    }

    // Called by Animation Event at the end of the attack animation
    public void endAttack()
    {
        if (parentAI != null)
            parentAI.endAttack();
    }

    // Called by Animation Event at the frame the web is released
    public void doWebAttack()
    {
        if (parentAI != null)
            parentAI.doWebAttack();
    }
}