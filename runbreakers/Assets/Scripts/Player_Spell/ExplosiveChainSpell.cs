using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class ExplosiveChainSpell : Player_Spell
{
    [Header("Projectile Settings")]
    [SerializeField] private GameObject lightningBoltPrefab;
    [SerializeField] private float projectileSpeed = 75f;

    [Header("Chain Settings")]
    [SerializeField] private int maxBounces = 5;
    [SerializeField] private float bounceRange = 12f;

    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] [Range(0f, 1f)] private float explosionDamageFalloff = 0.5f; // How much damage the explosion deals compared to the main hit

    [Header("Visuals")]
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private float lineDuration = 0.1f;
    [SerializeField] private LayerMask enemyLayer;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.enabled = false;
    }

    public override void Cast(Transform castPos, Vector3 direction)
    {
        GameObject bolt = Instantiate(lightningBoltPrefab, castPos.position, Quaternion.LookRotation(direction));
        bolt.GetComponent<Rigidbody>().linearVelocity = direction * projectileSpeed;

        Projectile projectile = bolt.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.SetDamage(Damage + Gamemanager.instance.playerScript.characterAttackPower);
            projectile.SetExplosiveChainSource(this);
            projectile.SetSpeed(projectileSpeed);
        }
    }

    public void InitiateBouncesAndExplosions(Vector3 hitPosition, Transform firstTarget, int initialDamage)
    {
        Explode(hitPosition, initialDamage);
        List<Transform> hitEnemies = new List<Transform>();
        hitEnemies.Add(firstTarget);
        StartCoroutine(Bounce(hitPosition, firstTarget, hitEnemies, maxBounces, initialDamage));
    }

    private IEnumerator Bounce(Vector3 fromPosition, Transform currentTarget, List<Transform> hitEnemies, int bouncesLeft, int currentDamage)
    {
        if (currentTarget == null) yield break;

        Vector3 currentTargetPosition = currentTarget.position;
        yield return StartCoroutine(DrawLine(fromPosition, currentTargetPosition));

        if (bouncesLeft <= 0) yield break;

        int nextDamage = currentDamage; // No damage falloff for main chain target

        Collider[] potentialTargets = Physics.OverlapSphere(currentTargetPosition, bounceRange, enemyLayer);
        Transform nextTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (var potentialTarget in potentialTargets)
        {
            if (!hitEnemies.Contains(potentialTarget.transform))
            {
                float distance = Vector3.Distance(currentTargetPosition, potentialTarget.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nextTarget = potentialTarget.transform;
                }
            }
        }

        if (nextTarget != null)
        {
            IDamage damageable = nextTarget.GetComponent<IDamage>();
            if (damageable != null)
            {
                hitEnemies.Add(nextTarget);
                damageable.takeDamage(nextDamage);
                Explode(nextTarget.position, nextDamage); // Explode on each new target
                yield return StartCoroutine(Bounce(currentTargetPosition, nextTarget, hitEnemies, bouncesLeft - 1, nextDamage));
            }
        }
    }

    private void Explode(Vector3 position, int damage)
    {
        if (explosionVFX != null)
        {
            Instantiate(explosionVFX, position, Quaternion.identity);
        }

        int explosionDamage = Mathf.RoundToInt(damage * (1 - explosionDamageFalloff));
        Collider[] colliders = Physics.OverlapSphere(position, explosionRadius);
        foreach (Collider hitCollider in colliders)
        {
            if (hitCollider.CompareTag("Player")) continue;

            IDamage damageable = hitCollider.GetComponent<IDamage>();
            if (damageable != null)
            {
                damageable.takeDamage(explosionDamage);
            }
        }
    }

    private IEnumerator DrawLine(Vector3 start, Vector3 end)
    {
        if (!float.IsFinite(start.x) || !float.IsFinite(start.y) || !float.IsFinite(start.z) ||
            !float.IsFinite(end.x) || !float.IsFinite(end.y) || !float.IsFinite(end.z))
        {
            yield break;
        }

        if (start == end) yield break;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.enabled = true;
        yield return new WaitForSeconds(lineDuration);
        lineRenderer.enabled = false;
    }
}
