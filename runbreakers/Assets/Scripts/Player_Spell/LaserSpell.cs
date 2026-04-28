
using UnityEngine;
using System.Collections.Generic;

public class LaserSpell : Player_Spell
{
    [SerializeField] private GameObject laserBulletPrefab;
    [SerializeField] private float projectileSpeed = 100f;

    public override void Cast(Transform castPos, Vector3 direction)
    {
        PlayCastSound();

        GameObject laser = Instantiate(laserBulletPrefab, castPos.position, Quaternion.LookRotation(direction));
        laser.GetComponent<Rigidbody>().linearVelocity = direction * projectileSpeed;

        Projectile projectile = laser.GetComponent<Projectile>();
        if (projectile != null)
        {
            int totalDamage = Damage + Gamemanager.instance.playerScript.characterAttackPower;
            projectile.SetDamage(totalDamage);
            projectile.SetSpeed(projectileSpeed);

            if (currentLevel >= 6)
            {
                projectile.SetBehavior(Projectile.ProjectileBehavior.Return, 0.75f);
            }
            else
            {
                projectile.SetBehavior(Projectile.ProjectileBehavior.Pierce);
            }
        }
    }

    protected override void OnLevelUp()
    {
        base.OnLevelUp();
        Damage++;
        Debug.Log(string.Format("{0} base damage increased to {1}!", this.name, Damage));
    }
}
