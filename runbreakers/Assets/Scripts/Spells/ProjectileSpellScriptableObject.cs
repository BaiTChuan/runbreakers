using UnityEngine;

[CreateAssetMenu]
public class ProjectileSpellScriptableObject : ScriptableObject
{
    [Header("---- Projectile Spell Stats ----")]
    public float lifeTime = 1f;
    public float speed = 15f;
    public float castSpeed = 0.5f;
    public float spellRadius = 0.5f;
}
