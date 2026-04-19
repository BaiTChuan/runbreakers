using UnityEngine;
using TMPro;

public class GoldUI : MonoBehaviour
{
    public static GoldUI instance;

    [SerializeField] TextMeshProUGUI goldText;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (goldText != null)
        {
            int startingGold = 0;

            if (Gamemanager.instance != null)
            {
                startingGold = Gamemanager.gold;
            }

            goldText.text = "Gold: " + startingGold;
        }
    }

    public void UpdateGold(int amount)
    {
        if (goldText != null)
        {
            goldText.text = "Gold: " + amount;
        }
    }
}