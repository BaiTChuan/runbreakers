using UnityEngine;
using System.Collections;

public class damage : MonoBehaviour
{
    enum Damagetype { bullet, stationary, DOT }

    [SerializeField] Damagetype type;
    [SerializeField] Rigidbody rb;

    [SerializeField] int damageAmount;
    [SerializeField] float damageRate;

    bool isDamaging;

    Vector3 direction;

    void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("Hit: " + other.gameObject.name + " | Tag: " + other.tag);

        if (other.isTrigger)
            return;

        IDamage dmg = other.GetComponent<IDamage>();

        if (type == Damagetype.stationary)
        {
            if (dmg != null)
            {
                Debug.Log("Regular Contact Damage Hit");
                dmg.takeDamage(damageAmount);
            }
        }
        else if (type == Damagetype.bullet)
        {
            if (ShouldDamageTarget(other, dmg))
            {

                dmg.takeDamage(damageAmount);
                Destroy(gameObject);
            }

            if (other.CompareTag("Wall"))
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
            return;

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null && type == Damagetype.DOT && !isDamaging)
        {
            StartCoroutine(damageOther(dmg));
        }
    }

    IEnumerator damageOther(IDamage d)
    {
        isDamaging = true;
        d.takeDamage(damageAmount);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }

    bool ShouldDamageTarget(Collider other, IDamage dmg)
    {
        if (dmg == null)
            return false;

        if (CompareTag("PlayerProjectile") && other.CompareTag("Enemy"))
            return true;

        if (CompareTag("EnemyProjectile") && other.CompareTag("Player"))
            return true;

        if (other.CompareTag("Destructables"))
            return true;

        return false;
    }

    public void ChangeDmgBasedOnStats (int amount)
    {
       damageAmount += amount;
    }
}