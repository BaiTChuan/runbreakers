
using UnityEngine;

public abstract class Player_Spell : MonoBehaviour
{
    [SerializeField] public float CastSpeed;
    [SerializeField] public int Damage;


    public abstract void Cast(Transform castPos, Vector3 direction);
}
