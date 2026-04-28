using UnityEngine;

public class enemyProjectile : MonoBehaviour
{
    [Header("---- Projectile Settings ----")]
    [SerializeField] float speed = 15f;
    [SerializeField] float travelDistance = 30f;
    [SerializeField] int damage = 1;

    bool hasHit;
    Vector3 targetPosition;

    void Start()
    {
        hasHit = false;
    }

    public void SetDirection(Vector3 direction)
    {
        direction = direction.normalized;

        if (direction == Vector3.zero)
            return;

        transform.rotation = Quaternion.LookRotation(direction);
        targetPosition = transform.position + (direction * travelDistance);
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) <= 0.05f)
            Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        if (other.isTrigger) return;

        if (other.CompareTag("Player"))
        {
            IDamage damageable = other.GetComponentInParent<IDamage>();
            if (damageable != null)
                damageable.takeDamage(damage);

            hasHit = true;
            Destroy(gameObject);
        }
    }
}