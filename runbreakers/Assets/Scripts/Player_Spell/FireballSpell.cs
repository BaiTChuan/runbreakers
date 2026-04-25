using UnityEngine;

public class FireballSpell : Player_Spell
{
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float projectileSpeed;

    [SerializeField] private float explosionRadius = 4f;
    [SerializeField, Range(0f, 1f)] private float explosionDamageModifier = 0.5f;

    public override void Cast(Transform castPos, Vector3 direction)
    {
        GameObject fireball = Instantiate(fireballPrefab, castPos.position, Quaternion.LookRotation(direction));
        fireball.GetComponent<Rigidbody>().linearVelocity = direction * projectileSpeed;

        Projectile projectile = fireball.GetComponent<Projectile>();
        if (projectile != null)
        {
            int totalDamage = Damage + Gamemanager.instance.playerScript.characterAttackPower;
            projectile.SetDamage(totalDamage);
            projectile.SetSpeed(projectileSpeed);

            if (currentLevel >= 6)
            {
                int explosionDamage = Mathf.RoundToInt(totalDamage * explosionDamageModifier);
                projectile.SetExplosion(explosionRadius, explosionDamage);
            }
        }
    }

    protected override void OnLevelUp()
    {
        base.OnLevelUp();
        CastSpeed = Mathf.Max(0.1f, CastSpeed - 0.05f);
        Debug.Log(string.Format("{0} cast speed improved to {1}!", this.name, CastSpeed));
    }
}