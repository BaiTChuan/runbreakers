
using UnityEngine;

public class LaserSpell : Player_Spell
{
    [SerializeField] private GameObject laserBulletPrefab;
    [SerializeField] private float projectileSpeed = 100f;

    public override void Cast(Transform castPos, Vector3 direction)
    {
        GameObject laserBullet = Instantiate(laserBulletPrefab, castPos.position, Quaternion.LookRotation(direction));
        laserBullet.GetComponent<Rigidbody>().linearVelocity = direction * projectileSpeed;

        Projectile projectile = laserBullet.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.SetDamage(Damage);
            projectile.SetPiercing(true);
        }
    }
}
