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
    [SerializeField] float shakeTime = 0.5f;
    [SerializeField] float openTime = 1f;



    public void takeDamage(int amount)
    {
        hp -= amount;
        if (hp <= 0)
            StartCoroutine(DestroySequence());
    }

    IEnumerator DestroySequence()
    {
        if (animator != null)
        {
            animator.Play("Chest_Shake");
            yield return new WaitForSeconds(shakeTime + openTime);
        }

        Die();
    }

    void Die()
    {

        if (destroySounds.Length > 0 && audioSource != null)
        {
            AudioClip clip = destroySounds[Random.Range(0, destroySounds.Length)];
            audioSource.PlayOneShot(clip);
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
