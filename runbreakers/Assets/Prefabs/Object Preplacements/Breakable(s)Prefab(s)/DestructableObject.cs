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
    [SerializeField] AudioClip[] destroySounds;
    [SerializeField] AudioSource audioSource;
    [SerializeField] Animator animator;



   // [SerializeField] bool isChest = false;



    public void takeDamage(int amount)
    {
        hp -= amount;
        if (hp <= 0)
            StartCoroutine(ShakeAndDie());
    }

    IEnumerator ShakeAndDie()
    {
        Vector3 originalPos = transform.position;
        float elapsed = 0f;
        float duration = 0.5f;
        float magnitude = 0.1f;

        while (elapsed < duration)
        {
            float x = originalPos.x + Random.Range(-magnitude, magnitude);
            float z = originalPos.z + Random.Range(-magnitude, magnitude);
            transform.position = new Vector3(x, originalPos.y, z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;
        Die();
    }


    void Die()
    {

        if (destroySounds.Length > 0)
        {
            AudioClip clip = destroySounds[Random.Range(0, destroySounds.Length)];
            AudioSource.PlayClipAtPoint(clip, transform.position, 1.8f);
        }

       
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
