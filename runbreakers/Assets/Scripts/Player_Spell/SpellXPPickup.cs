using UnityEngine;

public class SpellXPPickup : MonoBehaviour
{
    [SerializeField] public int xpAmount = 10;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IPickup pick = other.GetComponent<IPickup>();

            if (pick != null)
            {
                pick.getSpellXP(xpAmount);
                Destroy(gameObject);
            }
        }
    }
}
