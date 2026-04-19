
using UnityEngine;

public class FireballSpell : Player_Spell
{
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float projectileSpeed;

    public override void Cast(Transform castPos, Vector3 direction)
    {
        GameObject fireball = Instantiate(fireballPrefab, castPos.position, Quaternion.LookRotation(direction));
        fireball.GetComponent<Rigidbody>().linearVelocity = direction * projectileSpeed;

        Projectile projectile = fireball.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.SetDamage(Damage + Gamemanager.instance.playerScript.characterAttackPower);
        }
    }
}
