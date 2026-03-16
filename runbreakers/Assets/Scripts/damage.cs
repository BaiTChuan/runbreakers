using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class damage : MonoBehaviour
{
    enum damagetype { bullet, stationary, DOT }

    [SerializeField] damagetype type;
    [SerializeField] Rigidbody rb;

    [SerializeField] int damageAmount;
    [SerializeField] float damageRate;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;

    [SerializeField] ParticleSystem hitEffect;

    bool isDamaging;

    private Vector3 moveDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (type == damagetype.bullet)
        {
            rb.linearVelocity = transform.forward * speed;
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
            dmg.takeDamage(damageAmount);
        }

        else if (type == damagetype.bullet)
        {
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }

            dmg.takeDamage(damageAmount);
            Destroy(gameObject);
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

}
