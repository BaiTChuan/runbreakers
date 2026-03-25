using UnityEngine;

[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    [Header("---- Gun Model ----")]
    public GameObject gunModel;
    public GameObject bullet;

    [Header("---- Gun Stats ----")]
    public int shootDamage;
    public float shootRate;

    [Header("---- Gun Ammo ----")]
    public int ammoCur;
    public int ammoMax;

    [Header("---- Gun Effect ----")]
    public ParticleSystem shootEffect;

    [Header("---- Gun Sound ----")]
    public AudioClip[] shootSound;
    public float shootSoundVol;
}
