using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class ChainLightningSpell : Player_Spell
{
    [SerializeField] private GameObject lightningBoltPrefab;
    [SerializeField] private float projectileSpeed = 75f;

    [SerializeField] private int maxBounces = 3;
    [SerializeField] private float bounceRange = 10f;
    [SerializeField] [Range(0f, 1f)] private float damageFalloff = 0.2f;

    [SerializeField] private float lineDuration = 0.1f;
    [SerializeField] private LayerMask enemyLayer;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.enabled = false;
    }

    protected override void OnLevelUp()
    {
        base.OnLevelUp();
        if (currentLevel == 3 || currentLevel == 5)
        {
            maxBounces++;
        }
    }

    public override void Cast(Transform castPos, Vector3 direction)
    {
        GameObject bolt = Instantiate(lightningBoltPrefab, castPos.position, Quaternion.LookRotation(direction));
        bolt.GetComponent<Rigidbody>().linearVelocity = direction * projectileSpeed;

        Projectile projectile = bolt.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.SetDamage(Damage + Gamemanager.instance.playerScript.characterAttackPower);
            projectile.SetChainLightningSource(this);
            projectile.SetSpeed(projectileSpeed);
        }
    }

    public void ReceiveXpDirectly(int amount)
    {
        AddXp(amount);
    }

    public void InitiateBounces(Vector3 hitPosition, Transform firstTarget, int initialDamage)
    {
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

        int nextDamage = Mathf.RoundToInt(currentDamage * (1 - damageFalloff));

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
                yield return StartCoroutine(Bounce(currentTargetPosition, nextTarget, hitEnemies, bouncesLeft - 1, nextDamage));
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