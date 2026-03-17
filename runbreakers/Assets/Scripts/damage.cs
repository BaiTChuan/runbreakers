using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class damage : MonoBehaviour
{
    enum damagetype { bullet, stationary, DOT }
    enum projectileOwner { player, enemy }

    [SerializeField] damagetype type;

    [SerializeField] int damageAmount;
    [SerializeField] float damageRate;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;

    [SerializeField] ParticleSystem hitEffect;
    [SerializeField] projectileOwner owner;

    bool isDamaging;

    private Vector3 moveDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created.
    // (From Corey) I changed the bullets from working with Rigidbody body to use SetDirection() and moveDirection instead so the bullets weren't being moved twice.
void Start()
{
    if (type == damagetype.bullet)
    {
        Destroy(gameObject, destroyTime);
    }
}

    private void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        IDamage dmg = other.GetComponent<IDamage>();

        if (type == damagetype.stationary)
        {
            if (dmg != null)
            dmg.takeDamage(damageAmount);
        }

        else if (type == damagetype.bullet)
        {
            if (ShouldDamageTarget(other, dmg))
            {
            if (hitEffect != null)
                {
                    Instantiate(hitEffect, transform.position, Quaternion.identity);
                }
            
                dmg.takeDamage(damageAmount);
            Destroy(gameObject);
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
            return;

        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null && type == damagetype.DOT && !isDamaging)
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

    public void SetDirection(Vector3 dir)
    {
        moveDirection = dir.normalized;
        transform.rotation = Quaternion.LookRotation(moveDirection);
    }
    
     bool ShouldDamageTarget(Collider other, IDamage dmg)
    {
        if (dmg == null)
            return false;

        if (owner == projectileOwner.player && other.CompareTag("Enemy"))
            return true;

        if (owner == projectileOwner.enemy && other.CompareTag("Player"))
            return true;

        return false;
    }
}
