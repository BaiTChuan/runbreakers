using UnityEngine;

public class levelUpButton : MonoBehaviour
{
    [SerializeField] GameObject button;

    // Type 0 = health | 1 = speed | 2 = damage
    public int type;
    public int tier;

    bool rolled = false;

    // Update is called once per frame
    void Update()
    {
        if (Gamemanager.instance.isLevelUp == true && rolled == false)
        {
            rollUpgrade();
            rolled = true;
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

        if (type == 3)
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
