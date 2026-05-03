using UnityEngine;
using System.Collections.Generic;

public class Projectile : MonoBehaviour
{
    public enum ProjectileBehavior { StopOnHit, Pierce, Return }
    private ProjectileBehavior behavior = ProjectileBehavior.StopOnHit;

    private ChainLightningSpell chainSource = null;
    private ExplosiveChainSpell explosiveChainSource = null;
    private bool canExplode = false;
    private float explosionRadius;
    private int explosionDamage;
    private float returnDelay;
    private List<AudioClip> explosionSounds = new List<AudioClip>();
    private float explosionVolume = 1.0f;

    private int damage;
    private List<Collider> hitTargets = new List<Collider>();
    private float speed;
    private Rigidbody rb;
    private bool isReturning = false;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private GameObject explosionVFX;

    public void SetDamage(int amount)
    {
        damage = amount;
    }

    public void SetChainLightningSource(ChainLightningSpell source)
    {
        chainSource = source;
        behavior = ProjectileBehavior.StopOnHit;
    }

    public void SetExplosiveChainSource(ExplosiveChainSpell source)
    {
        explosiveChainSource = source;
        behavior = ProjectileBehavior.StopOnHit;
    }

    public void SetExplosion(float radius, int newExplosionDamage, List<AudioClip> sounds, float volume)
    {
        canExplode = true;
        explosionRadius = radius;
        explosionDamage = newExplosionDamage;
        explosionSounds = sounds;
        explosionVolume = volume;
    }

    public void SetBehavior(ProjectileBehavior newBehavior, float delay = 0)
    {
        behavior = newBehavior;
        if (behavior == ProjectileBehavior.Return)
        {
            returnDelay = delay;
        }
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        Destroy(gameObject, lifeTime);
    }

    private void Start()
    {
        if (behavior == ProjectileBehavior.Return)
        {
            Invoke(nameof(TriggerReturn), returnDelay);
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("Projectile hit: " + other.gameObject.name + " | Tag: " + other.tag);
        
        GameObject hitObject = other.gameObject;

        if (hitObject.CompareTag("Enemy"))
        {
            if (HandleDamage(other)) return;
        }

        if (hitObject.CompareTag("Destructables"))
        {
            if (HandleDamage(other)) return;
        }

        switch (behavior)
        {
            case ProjectileBehavior.StopOnHit:
                if (!other.isTrigger)
                {
                    if (canExplode) Explode();
                    Destroy(gameObject);
                }
                break;

            case ProjectileBehavior.Pierce:
                if (hitObject.CompareTag("Wall"))
                {
                    Destroy(gameObject);
                }
                break;

            case ProjectileBehavior.Return:
                if (isReturning && hitObject.CompareTag("Player"))
                {
                    Destroy(gameObject);
                }
                else if (!isReturning && hitObject.CompareTag("Wall"))
                {
                    TriggerReturn();
                }
                break;
        }
    }

    private void TriggerReturn()
    {
        if (isReturning) return;

        isReturning = true;
        rb.linearVelocity = -rb.linearVelocity;
        hitTargets.Clear();
    }

    private bool HandleDamage(Collider targetCollider)
    {
        if (targetCollider == null || hitTargets.Contains(targetCollider)) return false;

        IDamage damageable = targetCollider.GetComponent<IDamage>();
        if (damageable != null)
        {
            Debug.Log("Dealing " + damage + " damage to " + targetCollider.gameObject.name);
            damageable.takeDamage(damage);
            hitTargets.Add(targetCollider);
        }
        else
        {
            Debug.Log("NO IDamage found on " + targetCollider.gameObject.name);

        }

        {
    

        if (chainSource != null)
        {
            chainSource.InitiateBounces(transform.position, targetCollider.transform, damage);
            Destroy(gameObject);
            return true;
        }
        else if (explosiveChainSource != null)
        {
            explosiveChainSource.InitiateBouncesAndExplosions(transform.position, targetCollider.transform, damage);
            Destroy(gameObject);
            return true;
        }
        }
        return false;
    }


    private void Explode()
    {
        if (explosionVFX != null)
        {
            Instantiate(explosionVFX, transform.position, Quaternion.identity);
        }

        if (explosionSounds != null && explosionSounds.Count > 0)
        {
            AudioSource.PlayClipAtPoint(explosionSounds[Random.Range(0, explosionSounds.Count)], transform.position, explosionVolume);
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                continue;
            }
            IDamage damageable = hitCollider.GetComponent<IDamage>();
            if (damageable != null)
            {
                damageable.takeDamage(explosionDamage);
            }
        }
    }
}