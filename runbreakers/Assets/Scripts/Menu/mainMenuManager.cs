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
        healthCur.text = pHealthLevel.ToString();
        healthMax.text = pHealthMax.ToString();
        healthCostText.text = healthCost.ToString();

        damageCur.text = pDamageLevel.ToString();
        damageMax.text = pDamageMax.ToString();
        damageCostText.text = damageCost.ToString();

        speedCur.text = pSpeedLevel.ToString();
        speedMax.text = pSpeedMax.ToString();
        speedCostText.text = speedCost.ToString();

        luckCur.text = pLuckLevel.ToString();
        luckMax.text = pLuckMax.ToString();
        luckCostText.text = luckCost.ToString();

        armorCur.text = pArmorLevel.ToString();
        armorMax.text = pArmorMax.ToString();
        armorCostText.text = armorCost.ToString();

        castSpeedCur.text = pCastSpeedLevel.ToString();
        castSpeedMax.text = pCastSpeedMax.ToString();
        castSpeedCostText.text = castSpeedCost.ToString();

        reviveCur.text = pReviveLevel.ToString();
        reviveMax.text = pReviveMax.ToString();
        reviveCostText.text = reviveCost.ToString();

        rerollCur.text = pRerollLevel.ToString();
        rerollMax.text = pRerollMax.ToString();
        rerollCostText.text = rerollCost.ToString();

        goldCur.text = Gamemanager.gold.ToString();
    }

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
