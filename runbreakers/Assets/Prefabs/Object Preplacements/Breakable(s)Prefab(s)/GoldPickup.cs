using System.Collections;
using UnityEngine;

public class GoldPickup : MonoBehaviour, IPickup
{
    [SerializeField] public int goldAmount = 5;
    [SerializeField] AudioClip pickupSound;
    public bool fromChest = false;

    void Start()
    {
        if (fromChest)
            StartCoroutine(ArcUp());
    }
    IEnumerator ArcUp()
    {
        Vector3 startPos = transform.position;
        Vector3 peakPos = startPos + new Vector3(Random.Range(-1f, 1f), 2f, Random.Range(-1f, 1f));
        Vector3 landPos = new Vector3(peakPos.x, 0.5f, peakPos.z);

        float elapsed = 0f;
        float riseTime = 0.4f;

        while (elapsed < riseTime)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, peakPos, elapsed / riseTime);
            yield return null;

        }

        elapsed = 0f;
        float fallTime = 0.3f;

        while (elapsed < fallTime)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(peakPos, landPos, elapsed / fallTime);
            yield return null;
        }

        transform.position = landPos;
    }



    public void getBuff(buffStats buff) { }

    public void getGold(int amount)
    {
        if (Gamemanager.instance != null)
            Gamemanager.instance.AddGold(amount);

        if (GoldUI.instance != null)
            GoldUI.instance.UpdateGold(Gamemanager.gold);

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        
        IPickup pick = other.GetComponent<IPickup>();
        if (pick != null)
        {
            if (pickupSound != null)
                AudioSource.PlayClipAtPoint(pickupSound, transform.position, 2f);

            pick.getGold(goldAmount);
            Destroy(gameObject);
        }
    }

    public void getSpellXP(int amount) { }

}
