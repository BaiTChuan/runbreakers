using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class DestructableObject : MonoBehaviour, IDamage
{
    

    // This is the proper takeDamage function from IDamage Interface.
    
    [SerializeField] int hp = 1;
    [SerializeField] int minGold = 5;
    [SerializeField] int maxGold = 15;
    [SerializeField] GameObject GoldCoin;
    

   

    public void takeDamage(int amount)
    {
        hp -= amount;
        if (hp <= 0)
            Die();
    }

    void Die()
    {
       
        if (GoldCoin != null)
        {
            int goldToDrop = Random.Range(minGold, maxGold + 1);
            GameObject coin = Instantiate(GoldCoin, new Vector3(transform.position.x, 0.5f, transform.position.z), Quaternion.identity);
            GoldPickup pickup = coin.GetComponent<GoldPickup>();
            if (pickup != null)
                pickup.goldAmount = goldToDrop;

            
        }

        if (DestructableObjectsManager.instance != null)
            DestructableObjectsManager.instance.OnDestructableDestroyed();

        Destroy(gameObject);


    }
}
