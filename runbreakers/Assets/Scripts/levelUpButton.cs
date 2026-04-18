using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class levelUpButton : MonoBehaviour
{
    [SerializeField] GameObject button;
    [SerializeField] Image image;
    [SerializeField] TMP_Text text;

    // Type 0 = health | 1 = speed | 2 = damage
    public int type;
    public int tier;

    public Sprite healthImg;
    public Sprite speedImg;
    public Sprite damageImg;

    string health = "Health Up";
    string speed = "Speed Up";
    string damage = "Damage Up";

    bool rolled = false;

    // Update is called once per frame
    void Update()
    {
        if (Gamemanager.instance.isLevelUp == true && rolled == false)
        {
            rollUpgrade();
            rolled = true;

            if (type == 0)
            {
                image.sprite = healthImg;
                text.text = health;
            }

            if (type == 1)
            {
                image.sprite = speedImg;
                text.text = speed;
            }

            if (type == 2)
            {
                image.sprite = damageImg;
                text.text = damage;
            }
        }
    }

    public void upgrade()
    {
        if (type == 0)
        {
            if (tier == 0)
            {
                Gamemanager.instance.playerScript.hpLevelUp0();
                Gamemanager.instance.playerScript.updatePlayerUI();
            }
            if (tier == 1)
            {
                Gamemanager.instance.playerScript.hpLevelUp1();
                Gamemanager.instance.playerScript.updatePlayerUI();
            }
            if (tier == 2)
            {
                Gamemanager.instance.playerScript.hpLevelUp2();
                Gamemanager.instance.playerScript.updatePlayerUI();
            }
        }

        if (type == 1)
        {
            if (tier == 0)
            {
                Gamemanager.instance.playerScript.speedLevelUp0();
            }
            if (tier == 1)
            {
                Gamemanager.instance.playerScript.speedLevelUp1();
            }
            if (tier == 2)
            {
                Gamemanager.instance.playerScript.speedLevelUp2();
            }
        }

        if (type == 2)
        {
            if (tier == 0)
            {
                Gamemanager.instance.playerScript.damageLevelUp0();
            }
            if (tier == 1)
            {
                Gamemanager.instance.playerScript.damageLevelUp1();
            }
            if (tier == 2)
            {
                Gamemanager.instance.playerScript.damageLevelUp2();
            }
        }
        Gamemanager.instance.isLevelUp = false;
        rolled = false;
        Gamemanager.instance.stateUnpause();
    }

    void rollUpgrade()
    {
        type = Random.Range(0, 3);
        tier = Random.Range(0, 3);
    }
}
