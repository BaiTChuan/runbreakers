using UnityEngine;

[CreateAssetMenu]

public class buffStats : ScriptableObject
{
    public GameObject buffModel;
    [Header("----- BuffType ------")]
    [Header(" 0 = Health | 1 = Speed | 2 = Damage | 3 = Speed Debuff")]
    public int id;

    [Header("----- HealthBuff ------")]
    public int healAmount;

    [Header("----- SpeedBuff ------")]
    public float speedMultiplier;
    public int speedDuration;

    [Header("----- DamageBuff ------")]
    public float damageMultiplier;
    public int damageDuration;

    [Header("----- SpeedDebuff ------")]
    public float speedDownMultiplier;
    public int speedDownDuration;
}
