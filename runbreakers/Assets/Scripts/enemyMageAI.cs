using UnityEngine;

public class enemyMageAI : MonoBehaviour, IDamage
{
    [Header("---- Movement ----")]
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float stopDistance = 8f;
    [SerializeField] float retreatDistance = 4f;

    [Header("---- Attack ----")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform shootPoint;
    [SerializeField] float shootRate = 2f;

    [Header("---- Enemy Stats ----")]
    [SerializeField] int maxHP = 4;
    [SerializeField] int xpValue = 2;

    [Header ("---- Hit Effect ----")]
    [SerializeField] ParticleSystem beingHitEffect;

    int currentHP;
    float shootTimer;

    void Start()
    {
        currentHP = maxHP;
    }

    void Update()
    {
        if (gamemanager.instance == null || gamemanager.instance.player == null)
        return;

        shootTimer += Time.deltaTime;

        Vector3 SetDirection = gamemanager.instance.player.transform.position - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        if (distance > stopDistance)
        {
            transform.position += direction.normalized * moveSpeed * Time.deltaTime;
        }

        else if (distance < retreatDistance)
        {
            transform.position -= direction.normalized * moveSpeed * Time.deltaTime;
            TryShoot(direction);
        }
    }

    void TryShoot(Vector3 direction)
    {
        if (projectilePrefab == null || shootPoint == null)
        return;

        if (shootTimer < shootRate)
        return;

        shootTimer = 0f;

        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);

        damage dmgScript = projectile.GetComponent<damage>();

        if (dmgScript != null)
        {
            dmgScript.SetDirection(direction);
        }
    }

    public void takeDamage(int amount)
    {
        if (beingHitEffect != null)
        {
            beingHitEffect.Play();
        }

        currentHP -= amount;

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (gamemanager.instance == null || gamemanager.instance.player == null)
        return;

        playerXp xp = gamemanager.instance.player.GerComponent<playerXP>();

        if (xp != null)
        {
            xp.AddXP(xpValue);
        }

        Destory(gameObject);
    }
}