using UnityEngine;

public class GoldPickup : MonoBehaviour, IPickup
{
    [SerializeField] public int goldAmount = 5;

    public void getBuff(buffStats buff) { }
    public void getGun(gunStats gun) { }

    public void getGold(int amount)
    {
        if (Gamemanager.instance != null)
            Gamemanager.instance.AddGold(amount);

        if (GoldUI.instance != null)
            GoldUI.instance.UpdateGold(Gamemanager.instance.gold);

        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        
        IPickup pick = other.GetComponent<IPickup>();

        if (pick != null)
        {
            pick.getGold(goldAmount);
            Destroy(gameObject);
        }
    }

}
