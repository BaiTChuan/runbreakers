using UnityEngine;

public class broodEgg : MonoBehaviour, IDamage
{
    [Header("---- Egg Settings ----")]
    [SerializeField] GameObject broodMotherPrefab;
    [SerializeField] float hatchTime = 5f;
    [SerializeField] int maxHP = 3;

    [Header("---- Hit Effect ----")]
    [SerializeField] ParticleSystem beingHitEffect;

    int currentHP;
    float hatchTimer;
    bool hasHatched;

    void Start()
    {
        currentHP = maxHP;
        hatchTimer = 0f;
        hasHatched = false;
    }

    void Update()
    {
        if (hasHatched)
            return;

        hatchTimer += Time.deltaTime;

        if (hatchTimer >= hatchTime)
        {
            hatch();
        }
    }

    public void takeDamage(int amount)
    {
        if (hasHatched)
            return;

        if (beingHitEffect != null)
        {
            beingHitEffect.Play();
        }

        currentHP -= amount;

        if (currentHP <= 0)
        {
            Destroy(gameObject);
        }
    }

    void hatch()
    {
        if (hasHatched)
            return;

        hasHatched = true;

        if (broodMotherPrefab != null)
        {
            Instantiate(broodMotherPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
