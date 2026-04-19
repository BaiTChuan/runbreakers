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
        goldText.text = "Gold: 0";

    }

    public void UpdateGold(int amount)
    {
        goldText.text = "Gold: " + amount;
    }

   

}
