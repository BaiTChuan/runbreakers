using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class mainMenuManager : MonoBehaviour
{
    public static mainMenuManager instance;

    public GameObject menuActive;
    public GameObject credits;
    public GameObject permanentShop;
    public GameObject settings;
    public GameObject startScreen;
    public GameObject prevMenu;

    [SerializeField] TMP_Text goldCur;

    [Header("----- HealthP ------")]
    [SerializeField] TMP_Text healthCur;
    [SerializeField] TMP_Text healthMax;
    public static int pHealthLevel = 0;
    static int pHealthMax = 3;
    [SerializeField] TMP_Text healthCostText;
    [SerializeField] int healthCost;
    [SerializeField] int healthLevelStat = 4;

    [Header("----- DamageP ------")]
    [SerializeField] TMP_Text damageCur;
    [SerializeField] TMP_Text damageMax;
    public static int pDamageLevel = 0;
    static int pDamageMax = 3;
    [SerializeField] TMP_Text damageCostText;
    [SerializeField] int damageCost;
    [SerializeField] int damageLevelStat = 2;

    [Header("----- SpeedP ------")]
    [SerializeField] TMP_Text speedCur;
    [SerializeField] TMP_Text speedMax;
    public static int pSpeedLevel = 0;
    static int pSpeedMax = 3;
    [SerializeField] TMP_Text speedCostText;
    [SerializeField] int speedCost;
    [SerializeField] float speedLevelStat = 1;

    [Header("----- LuckP ------")]
    [SerializeField] TMP_Text luckCur;
    [SerializeField] TMP_Text luckMax;
    public static int pLuckLevel = 0;
    static int pLuckMax = 3;
    [SerializeField] TMP_Text luckCostText;
    [SerializeField] int luckCost;
    [SerializeField]int luckLevelStat = 1;

    [Header("----- ArmorP ------")]
    [SerializeField] TMP_Text armorCur;
    [SerializeField] TMP_Text armorMax;
    public static int pArmorLevel = 0;
    static int pArmorMax = 3;
    [SerializeField] TMP_Text armorCostText;
    [SerializeField] int armorCost;
    [SerializeField] int armorLevelStat = 1;

    [Header("----- CastSpeedP ------")]
    [SerializeField] TMP_Text castSpeedCur;
    [SerializeField] TMP_Text castSpeedMax;
    public static int pCastSpeedLevel = 0;
    static int pCastSpeedMax = 3;
    [SerializeField] TMP_Text castSpeedCostText;
    [SerializeField] int castSpeedCost;
    [SerializeField] float castSpeedLevelStat = 0.05f;

    [Header("----- ReviveP ------")]
    [SerializeField] TMP_Text reviveCur;
    [SerializeField] TMP_Text reviveMax;
    public static int pReviveLevel = 0;
    static int pReviveMax = 1;
    [SerializeField] TMP_Text reviveCostText;
    [SerializeField] int reviveCost;
    [SerializeField] int reviveLevelStat = 1;

    [Header("----- RerollP ------")]
    [SerializeField] TMP_Text rerollCur;
    [SerializeField] TMP_Text rerollMax;
    public static int pRerollLevel = 0;
    static int pRerollMax = 3;
    [SerializeField] TMP_Text rerollCostText;
    [SerializeField] int rerollCost;
    [SerializeField] int rerollLevelStat = 1;

    [Header("----- Levels ------")]
    public GameObject healthUp;
    public GameObject healthUp2;
    public GameObject healthUp3;

    public GameObject speedUp;
    public GameObject speedUp2;
    public GameObject speedUp3;

    public GameObject damageUp;
    public GameObject damageUp2;
    public GameObject damageUp3;

    public GameObject castSpeedUp;
    public GameObject castSpeedUp2;
    public GameObject castSpeedUp3;

    public GameObject armorUp;
    public GameObject armorUp2;
    public GameObject armorUp3;

    public GameObject reviveUp;

    public GameObject rerollUp;
    public GameObject rerollUp2;

    public GameObject luckUp;
    public GameObject luckUp2;
    public GameObject luckUp3;

    static bool firstTime = true;
    public bool dataDeleted = false;

    public static int healthP = 0;
    public static float speedP = 0;
    public static int damageP = 0;
    public static int luckP = 0;
    public static float castSpeedP = 0;
    public static int armorP = 0;
    public static int reviveP = 0;
    public static int rerollP = 0;

    public static float masterVol = 1;
    public static float sfxVol = 1;
    public static float musicVol = 1;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (firstTime == true)
        {
            loadStats();
            menuActive = startScreen;
            firstTime = false;
        }
        else
        {
            menuActive = null;
        }

        updateShop();
    }

    void loadStats()
    {
        healthP = PlayerPrefs.GetInt("Health", 0);
        speedP = PlayerPrefs.GetFloat("Speed", 0);
        damageP = PlayerPrefs.GetInt("Damage", 0);
        luckP = PlayerPrefs.GetInt("Luck", 0);
        castSpeedP = PlayerPrefs.GetFloat("CastSpeed", 0);
        armorP = PlayerPrefs.GetInt("Armor", 0);
        reviveP = PlayerPrefs.GetInt("Revive", 0);
        rerollP = PlayerPrefs.GetInt("Reroll", 0);

        pHealthLevel = PlayerPrefs.GetInt("HealthL", 0);
        pSpeedLevel = PlayerPrefs.GetInt("SpeedL", 0);
        pDamageLevel = PlayerPrefs.GetInt("DamageL", 0);
        pLuckLevel = PlayerPrefs.GetInt("LuckL", 0);
        pCastSpeedLevel = PlayerPrefs.GetInt("CastSpeedL", 0);
        pArmorLevel = PlayerPrefs.GetInt("ArmorL", 0);
        pReviveLevel = PlayerPrefs.GetInt("ReviveL", 0);
        pRerollLevel = PlayerPrefs.GetInt("RerollL", 0);

        masterVol = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
        sfxVol = PlayerPrefs.GetFloat("SFX", 0.5f);
        musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.5f);

        Gamemanager.gold = PlayerPrefs.GetInt("Gold", 0);
    }

    // Update is called once per frame
    void Update()
    {

        if (menuActive == null && prevMenu != null)
        {
            prevMenu.SetActive(false);
        }
        else if (menuActive != null)
        {
            menuActive.SetActive(true);
        }

        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive != null)
            {
                prevMenu = menuActive;
                menuActive = null;
            }
        }

        if (dataDeleted == true)
        {
            updateShop();
            dataDeleted = false;
        }
    }

    public void updateShop()
    {
        #region
        healthCostText.text = healthCost.ToString();

        damageCostText.text = damageCost.ToString();

        speedCostText.text = speedCost.ToString();

        luckCostText.text = luckCost.ToString();

        armorCostText.text = armorCost.ToString();

        castSpeedCostText.text = castSpeedCost.ToString();

        reviveCostText.text = reviveCost.ToString();

        rerollCostText.text = rerollCost.ToString();
        #endregion

        #region

        if (pHealthLevel == 0)
        {
            healthUp.gameObject.SetActive(false);
            healthUp2.gameObject.SetActive(false);
            healthUp3.gameObject.SetActive(false);
        }
        else if (pHealthLevel == 1)
        {
            healthUp.gameObject.SetActive(true);
            healthUp2.gameObject.SetActive(false);
            healthUp3.gameObject.SetActive(false);
        }
        else if (pHealthLevel == 2)
        {
            healthUp.gameObject.SetActive(true);
            healthUp2.gameObject.SetActive(true);
            healthUp3.gameObject.SetActive(false);
        }
        else if (pHealthLevel == 3)
        {
            healthUp.gameObject.SetActive(true);
            healthUp2.gameObject.SetActive(true);
            healthUp3.gameObject.SetActive(true);
        }

        if (pSpeedLevel == 0)
        {
            speedUp.gameObject.SetActive(false);
            speedUp2.gameObject.SetActive(false);
            speedUp3.gameObject.SetActive(false);
        }
        else if (pSpeedLevel == 1)
        {
            speedUp.gameObject.SetActive(true);
            speedUp2.gameObject.SetActive(false);
            speedUp3.gameObject.SetActive(false);
        }
        else if (pSpeedLevel == 2)
        {
            speedUp.gameObject.SetActive(true);
            speedUp2.gameObject.SetActive(true);
            speedUp3.gameObject.SetActive(false);
        }
        else if (pSpeedLevel == 3)
        {
            speedUp.gameObject.SetActive(true);
            speedUp2.gameObject.SetActive(true);
            speedUp3.gameObject.SetActive(true);
        }

        if (pArmorLevel == 0)
        {
            armorUp.gameObject.SetActive(false);
            armorUp2.gameObject.SetActive(false);
            armorUp3.gameObject.SetActive(false);
        }
        else if (pArmorLevel == 1)
        {
            armorUp.gameObject.SetActive(true);
            armorUp2.gameObject.SetActive(false);
            armorUp3.gameObject.SetActive(false);
        }
        else if (pArmorLevel == 2)
        {
            armorUp.gameObject.SetActive(true);
            armorUp2.gameObject.SetActive(true);
            armorUp3.gameObject.SetActive(false);
        }
        else if (pArmorLevel == 3)
        {
            armorUp.gameObject.SetActive(true);
            armorUp2.gameObject.SetActive(true);
            armorUp3.gameObject.SetActive(true);
        }

        if (pDamageLevel == 0)
        {
            damageUp.gameObject.SetActive(false);
            damageUp2.gameObject.SetActive(false);
            damageUp3.gameObject.SetActive(false);
        }
        else if (pDamageLevel == 1)
        {
            damageUp.gameObject.SetActive(true);
            damageUp2.gameObject.SetActive(false);
            damageUp3.gameObject.SetActive(false);
        }
        else if (pDamageLevel == 2)
        {
            damageUp.gameObject.SetActive(true);
            damageUp2.gameObject.SetActive(true);
            damageUp3.gameObject.SetActive(false);
        }
        else if (pDamageLevel == 3)
        {
            damageUp.gameObject.SetActive(true);
            damageUp2.gameObject.SetActive(true);
            damageUp3.gameObject.SetActive(true);
        }

        if (pLuckLevel == 0)
        {
            luckUp.gameObject.SetActive(false);
            luckUp2.gameObject.SetActive(false);
            luckUp3.gameObject.SetActive(false);
        }
        else if (pLuckLevel == 1)
        {
            luckUp.gameObject.SetActive(true);
            luckUp2.gameObject.SetActive(false);
            luckUp3.gameObject.SetActive(false);
        }
        else if (pLuckLevel == 2)
        {
            luckUp.gameObject.SetActive(true);
            luckUp2.gameObject.SetActive(true);
            luckUp3.gameObject.SetActive(false);
        }
        else if (pLuckLevel == 3)
        {
            luckUp.gameObject.SetActive(true);
            luckUp2.gameObject.SetActive(true);
            luckUp3.gameObject.SetActive(true);
        }

        if (pReviveLevel == 0)
        {
            reviveUp.gameObject.SetActive(false);
        }
        else if (pReviveLevel == 1)
        {
            reviveUp.gameObject.SetActive(true);
        }

        if (pRerollLevel == 0)
        {
            rerollUp.gameObject.SetActive(false);
            rerollUp2.gameObject.SetActive(false);
        }
        else if (pRerollLevel == 1)
        {
            rerollUp.gameObject.SetActive(true);
            rerollUp2.gameObject.SetActive(false);
        }
        else if (pRerollLevel == 2)
        {
            rerollUp.gameObject.SetActive(true);
            rerollUp2.gameObject.SetActive(true);
        }

        if (pCastSpeedLevel == 0)
        {
            castSpeedUp.gameObject.SetActive(false);
            castSpeedUp2.gameObject.SetActive(false);
            castSpeedUp3.gameObject.SetActive(false);
        }
        else if (pCastSpeedLevel == 1)
        {
            castSpeedUp.gameObject.SetActive(true);
            castSpeedUp2.gameObject.SetActive(false);
            castSpeedUp3.gameObject.SetActive(false);
        }
        else if (pCastSpeedLevel == 2)
        {
            castSpeedUp.gameObject.SetActive(true);
            castSpeedUp2.gameObject.SetActive(true);
            castSpeedUp3.gameObject.SetActive(false);
        }
        else if (pCastSpeedLevel == 3)
        {
            castSpeedUp.gameObject.SetActive(true);
            castSpeedUp2.gameObject.SetActive(true);
            castSpeedUp3.gameObject.SetActive(true);
        }

        #endregion
        goldCur.text = Gamemanager.gold.ToString();
    }

    #region PermanentUpgrades

    public void pHealthUp()
    {
        if (pHealthLevel < pHealthMax && (Gamemanager.gold >= healthCost))
        {
            Gamemanager.gold -= healthCost;
            pHealthLevel++;
            healthP += healthLevelStat;
            updateShop();
        }
    }
    public void pSpeedUp()
    {
        if (pSpeedLevel < pSpeedMax && (Gamemanager.gold >= speedCost))
        {
            Gamemanager.gold -= healthCost;
            pSpeedLevel++;
            speedP += speedLevelStat;
            updateShop();
        }
    }

    public void pDamageUp()
    {
        if (pDamageLevel < pDamageMax && (Gamemanager.gold >= damageCost))
        {
            Gamemanager.gold -= damageCost;
            pDamageLevel++;
            damageP += damageLevelStat;
            updateShop();
        }
    }

    public void pLuckUp()
    {
        if (pLuckLevel < pLuckMax && (Gamemanager.gold >= luckCost))
        {
            Gamemanager.gold -= luckCost;
            pLuckLevel++;
            luckP = luckP + luckLevelStat;
            updateShop();
        }
    }

    public void pArmorUp()
    {
        if (pArmorLevel < pArmorMax && (Gamemanager.gold >= armorCost))
        {
            Gamemanager.gold -= armorCost;
            pArmorLevel++;
            armorP += armorLevelStat;
            updateShop();
        }
    }
    public void pCastSpeedUp()
    {
        if (pCastSpeedLevel < pCastSpeedMax && (Gamemanager.gold >= castSpeedCost))
        {
            Gamemanager.gold -= castSpeedCost;
            pCastSpeedLevel++;
            castSpeedP += castSpeedLevelStat;
            updateShop();
        }
    }

    public void pReviveUp()
    {
        if (pReviveLevel < pReviveMax && (Gamemanager.gold >= reviveCost))
        {
            Gamemanager.gold -= reviveCost;
            pReviveLevel++;
            reviveP += reviveLevelStat;
            updateShop();
        }
    }

    public void pRerollUp()
    {
        if (pRerollLevel < pRerollMax && (Gamemanager.gold >= rerollCost))
        {
            Gamemanager.gold -= rerollCost;
            pRerollLevel++;
            rerollP += rerollLevelStat;
            updateShop();
        }
    }
}
#endregion