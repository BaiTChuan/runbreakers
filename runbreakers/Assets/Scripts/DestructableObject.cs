using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DestructableObject : MonoBehaviour, IDamage
{
    // This is the proper takeDamage function from IDamage Interface.

    [SerializeField] int hp = 1;
    [SerializeField] int goldReward = 10;

    public void takeDamage(int amount)
    {
        Destroy(gameObject); // Destroy after

       // throw new System.NotImplementedException();
    }

    void Die()
    {
        if (Gamemanager.instance != null)
            Gamemanager.instance.AddGold(goldReward);

    }
}
