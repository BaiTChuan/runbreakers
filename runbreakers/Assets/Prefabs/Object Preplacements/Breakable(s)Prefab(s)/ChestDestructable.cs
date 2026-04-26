using UnityEngine;
using System.Collections;

public class ChestDestructable : MonoBehaviour, IDamage
{
    [SerializeField] int hp = 3;
    [SerializeField] int minGold = 30;
    [SerializeField] int maxGold = 60;
    [SerializeField] GameObject gold;
    [SerializeField] AudioClip[] openSounds;
    [SerializeField] Animator animator;
    [SerializeField] float shakeTime = 1.3f;
    [SerializeField] float openTime = 2.3f;

    bool isDead = false;

    public void takeDamage(int amount)
    {
       if (isDead) return;
        hp -= amount;
        if (hp <= 0)
            StartCoroutine(OpenSequence());
        

    }

    IEnumerator OpenSequence()
    {
        isDead = true;

        if (animator != null)
        {
            animator.Play("Chest_Shake");
            yield return new WaitForSeconds(shakeTime + openTime);
        }

        if (openSounds.Length > 0)
            AudioSource.PlayClipAtPoint(openSounds[Random.Range(0, openSounds.Length)], transform.position, 1f);

        if (gold != null)
        {
            int goldToDrop = Random.Range(minGold, maxGold + 1);
            GameObject coin = Instantiate(gold, new Vector3(transform.position.x, 0.5f, transform.position.z), Quaternion.identity);
            GoldPickup pickup = coin.GetComponentInChildren<GoldPickup>();
            if (pickup != null)
                pickup.goldAmount = goldToDrop;
        }

        if (DestructableObjectsManager.instance != null)
            DestructableObjectsManager.instance.OnDestructableDestroyed();

        Destroy(gameObject, 3f);
    }





}
