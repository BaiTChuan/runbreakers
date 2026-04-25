using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class levelUpButton : MonoBehaviour
{
    [SerializeField] GameObject button;
    [SerializeField] Image image;
    [SerializeField] Image tierImage;
    [SerializeField] TMP_Text text;

    // Type 0 = health | 1 = speed | 2 = damage | 3 = armor | 4 = cast speed | 5 = luck
    public int type;
    public int tier;

    public Sprite healthImg;
    public Sprite speedImg;
    public Sprite damageImg;
    public Sprite armorImg;
    public Sprite luckImg;
    public Sprite castSpeedImg;

    public Sprite tier1;
    public Sprite tier2;
    public Sprite tier3;

    string health = "Health Up";
    string speed = "Speed Up";
    string damage = "Damage Up";
    string armor = "Armor Up";
    string castSpeed = "Cast Speed Up";
    string luck = "Luck Up";

    string health2 = "Health Up 2";
    string speed2 = "Speed Up 2";
    string damage2 = "Damage Up 2";
    string armor2 = "Armor Up 2";
    string castSpeed2 = "Cast Speed Up 2";
    string luck2 = "Luck Up 2";

    string health3 = "Health Up 3";
    string speed3 = "Speed Up 3";
    string damage3 = "Damage Up 3";
    string armor3 = "Armor Up 3";
    string castSpeed3 = "Cast Speed Up 3";
    string luck3 = "Luck Up 3";

    bool rolled = false;

    // Update is called once per frame
    void Update()
    {
        if (Gamemanager.instance.isLevelUp == true && rolled == false)
        {
            rollUpgrade();
            rolled = true;
        }

        if (Gamemanager.instance.isRerolled == true)
        {
            rollUpgrade();
        }
    }

    public void upgrade()
    {
        if (Gamemanager.instance.rolling == false)
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

            if (type == 3)
            {
                if (tier == 0)
                {
                    Gamemanager.instance.playerScript.armorLevelUp0();
                }
                if (tier == 1)
                {
                    Gamemanager.instance.playerScript.armorLevelUp1();
                }
                if (tier == 2)
                {
                    Gamemanager.instance.playerScript.armorLevelUp2();
                }
            }

            if (type == 4)
            {
                if (tier == 0)
                {
                    Gamemanager.instance.playerScript.castSpeedLevelUp0();
                }
                if (tier == 1)
                {
                    Gamemanager.instance.playerScript.castSpeedLevelUp1();
                }
                if (tier == 2)
                {
                    Gamemanager.instance.playerScript.castSpeedLevelUp2();
                }
            }

            if (type == 5)
            {
                if (tier == 0)
                {
                    Gamemanager.instance.playerScript.luckLevelUp0();
                }
                if (tier == 1)
                {
                    Gamemanager.instance.playerScript.luckLevelUp1();
                }
                if (tier == 2)
                {
                    Gamemanager.instance.playerScript.luckLevelUp2();
                }
            }

            Gamemanager.instance.isLevelUp = false;
            rolled = false;
            Gamemanager.instance.stateUnpause();
        }
    }

    void rollUpgrade()
    {
        type = Random.Range(0, 6);
        tier = Random.Range(0, 100);

        if (tier >= Gamemanager.instance.tier1Min && tier <= Gamemanager.instance.tier1Max)
        {
            tierImage.sprite = tier1;
        }
        if (tier >= Gamemanager.instance.tier2Min && tier <= Gamemanager.instance.tier2Max)
        {
            tierImage.sprite = tier2;
        }
        if (tier >= Gamemanager.instance.tier3Min && tier <= Gamemanager.instance.tier3Max)
        {
            tierImage.sprite = tier3;
        }

        if (type == 0)
        {
            image.sprite = healthImg;
            if (tier >= Gamemanager.instance.tier1Min && tier <= Gamemanager.instance.tier1Max)
            {
                text.text = health;
            }
            if (tier >= Gamemanager.instance.tier2Min && tier <= Gamemanager.instance.tier2Max)
            {
                text.text = health2;
            }
            if (tier >= Gamemanager.instance.tier3Min && tier <= Gamemanager.instance.tier3Max)
            {
                text.text = health3;
            }
        }

        if (type == 1)
        {
            image.sprite = speedImg;
            if (tier >= Gamemanager.instance.tier1Min && tier <= Gamemanager.instance.tier1Max)
            {
                text.text = speed;
            }
            if (tier >= Gamemanager.instance.tier2Min && tier <= Gamemanager.instance.tier2Max)
            {
                text.text = speed2;
            }
            if (tier >= Gamemanager.instance.tier3Min && tier <= Gamemanager.instance.tier3Max)
            {
                text.text = speed3;
            }
        }

        if (type == 2)
        {
            image.sprite = damageImg;
            if (tier >= Gamemanager.instance.tier1Min && tier <= Gamemanager.instance.tier1Max)
            {
                text.text = damage;
            }
            if (tier >= Gamemanager.instance.tier2Min && tier <= Gamemanager.instance.tier2Max)
            {
                text.text = damage2;
            }
            if (tier >= Gamemanager.instance.tier3Min && tier <= Gamemanager.instance.tier3Max)
            {
                text.text = damage3;
            }
        }

        if (type == 3)
        {
            image.sprite = armorImg;
            if (tier >= Gamemanager.instance.tier1Min && tier <= Gamemanager.instance.tier1Max)
            {
                text.text = armor;
            }
            if (tier >= Gamemanager.instance.tier2Min && tier <= Gamemanager.instance.tier2Max)
            {
                text.text = armor2;
            }
            if (tier >= Gamemanager.instance.tier3Min && tier <= Gamemanager.instance.tier3Max)
            {
                text.text = armor3;
            }
        }

        if (type == 4)
        {
            image.sprite = castSpeedImg;
            if (tier >= Gamemanager.instance.tier1Min && tier <= Gamemanager.instance.tier1Max)
            {
                text.text = castSpeed;
            }
            if (tier >= Gamemanager.instance.tier2Min && tier <= Gamemanager.instance.tier2Max)
            {
                text.text = castSpeed2;
            }
            if (tier >= Gamemanager.instance.tier3Min && tier <= Gamemanager.instance.tier3Max)
            {
                text.text = castSpeed3;
            }
        }

        if (type == 5)
        {
            image.sprite = luckImg;
            if (tier >= Gamemanager.instance.tier1Min && tier <= Gamemanager.instance.tier1Max)
            {
                text.text = luck;
            }
            if (tier >= Gamemanager.instance.tier2Min && tier <= Gamemanager.instance.tier2Max)
            {
                text.text = luck2;
            }
            if (tier >= Gamemanager.instance.tier3Min && tier <= Gamemanager.instance.tier3Max)
            {
                text.text = luck3;
            }
        }
    }
}
