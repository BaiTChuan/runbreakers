using UnityEngine;

public class GoldPickup : MonoBehaviour, IPickup
{
    [SerializeField] public int goldAmount = 5;
    [SerializeField] AudioClip pickupSound;

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
