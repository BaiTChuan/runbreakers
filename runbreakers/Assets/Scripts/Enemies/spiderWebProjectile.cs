using UnityEngine;

public class spiderWebProjectile : MonoBehaviour
{
    [Header("---- Projectile Settings ----")]
    [SerializeField] float speed = 8f;
    [SerializeField] float travelDistance = 12f;
    [SerializeField] float stoppedLifetime = 6f;

    [Header("---- Debuff ----")]
    [SerializeField] buffStats webDebuff;

    bool hasHit;
    bool hasStopped;

    Vector3 targetPosition;

    void Start()
    {
        hasHit = false;
        hasStopped = false;
    }

    void Update()
    {
        if (hasStopped)
            return;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) <= 0.05f)
        {
            stopProjectile();
        }
    }

    public void SetDirection(Vector3 direction)
    {
        direction = direction.normalized;

        if (direction == Vector3.zero)
            return;

        transform.rotation = Quaternion.LookRotation(direction);
        targetPosition = transform.position + (direction * travelDistance);
        Debug.Log("Web direction: " + direction + " | targetPosition: " + targetPosition);
    }

    void stopProjectile()
    {
        if (hasStopped)
            return;

        hasStopped = true;
        Destroy(gameObject, stoppedLifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasHit)
            return;

        if (other.isTrigger)
            return;

        if (other.CompareTag("Player"))
        {
            playerControl buffTarget = other.GetComponentInParent<playerControl>();

            if (buffTarget != null && webDebuff != null)
            {
                buffTarget.getBuff(webDebuff);
            }

            hasHit = true;
            Destroy(gameObject);
        }
    }
}