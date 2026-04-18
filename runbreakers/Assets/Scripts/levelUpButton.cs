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

    string health2 = "Health Up 2";
    string speed2 = "Speed Up 2";
    string damage2 = "Damage Up 2";

    string health3 = "Health Up 3";
    string speed3 = "Speed Up 3";
    string damage3 = "Damage Up 3";

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
                if (tier == 0)
                {
                    text.text = health;
                }
                if (tier == 1)
                {
                    text.text = health2;
                }
                if (tier == 2)
                {
                    text.text = health3;
                }
            }

            if (type == 1)
            {
                image.sprite = speedImg;
                if (tier == 0)
                {
                    text.text = speed;
                }
                if (tier == 1)
                {
                    text.text = speed2;
                }
                if (tier == 2)
                {
                    text.text = speed3;
                }
            }

            if (type == 2)
            {
                image.sprite = damageImg;
                if (tier == 0)
                {
                    text.text = damage;
                }
                if (tier == 1)
                {
                    text.text = damage2;
                }
                if (tier == 2)
                {
                    text.text = damage3;
                }
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
