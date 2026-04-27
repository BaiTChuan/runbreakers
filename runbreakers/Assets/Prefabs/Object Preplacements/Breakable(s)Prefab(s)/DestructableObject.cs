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
    [SerializeField] float shakeMagnitude = 0.1f;
    [SerializeField] float shakeSpeed = 25f;

   // [SerializeField] bool isChest = false;



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
        else
        {
            yield return StartCoroutine(Shake());
        }

            Die();
    }

    IEnumerator Shake()
    {
        Vector3 originalPos = transform.position;
        float timer = 0f;

        while (timer < shakeTime)
        {
            timer += Time.deltaTime;
            float x = Mathf.Sin(timer * shakeSpeed) * shakeMagnitude;
            float z = Mathf.Sin(timer * shakeSpeed * 0.7f) * shakeMagnitude;
            transform.position = originalPos + new Vector3(x, 0f, z);
            yield return null;
        }

        transform.position = originalPos;
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
