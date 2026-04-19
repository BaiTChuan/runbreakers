
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private int damage;
    private bool isPiercing = false;
    private ChainLightningSpell chainSource = null;
    [SerializeField] private float lifeTime = 5f;

    public void SetDamage(int amount)
    {
        damage = amount;
    }

    public void SetPiercing(bool piercing)
    {
        isPiercing = piercing;
    }

    public void SetChainLightningSource(ChainLightningSpell source)
    {
        chainSource = source;
    }

    private void Awake()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamage damageable = other.GetComponent<IDamage>();
        if (damageable != null)
        {
            damageable.takeDamage(damage);
        }

        if (chainSource != null && damageable != null)
        {
            chainSource.InitiateBounces(other.transform, damage);
            Destroy(gameObject);
            return;
        }

        if (isPiercing && damageable != null)
        {
            return;
        }

        if (other.isTrigger == false)
        {
            Destroy(gameObject);
        }
    }
}
