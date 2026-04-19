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
    [SerializeField] TMP_Text healthCost;
    public static int pHealthLevel = 0;
    static int pHealthMax = 3;
    [SerializeField] int healthCostAmount;
    [SerializeField] int healthLevelStat = 4;

    [Header("----- DamageP ------")]
    [SerializeField] TMP_Text damageCur;
    [SerializeField] TMP_Text damageMax;
    [SerializeField] TMP_Text damageCost;
    public static int pDamageLevel = 0;
    static int pDamageMax = 3;
    [SerializeField] int damageCostAmount;
    [SerializeField] int damageLevelStat = 2;

    [Header("----- SpeedP ------")]
    [SerializeField] TMP_Text speedCur;
    [SerializeField] TMP_Text speedMax;
    [SerializeField] TMP_Text speedCost;
    public static int pSpeedLevel = 0;
    static int pSpeedMax = 3;
    [SerializeField] int speedCostAmount;
    [SerializeField] float speedLevelStat = 1;

    [Header("----- ArmorP ------")]
    [SerializeField] TMP_Text armorCur;
    [SerializeField] TMP_Text armorMax;
    [SerializeField] TMP_Text armorCost;
    public static int pArmorLevel = 0;
    static int pArmorMax = 3;
    [SerializeField] int armorCostAmount;
    [SerializeField] int armorLevelStat = 1;

    [Header("----- CastSpeedP ------")]
    [SerializeField] TMP_Text castSpeedCur;
    [SerializeField] TMP_Text castSpeedMax;
    [SerializeField] TMP_Text castSpeedCost;
    public static int pCastSpeedLevel = 0;
    static int pCastSpeedMax = 3;
    [SerializeField] int castSpeedCostAmount;
    [SerializeField] float castSpeedLevelStat = 0.05f;

    [Header("----- LuckP ------")]
    [SerializeField] TMP_Text luckCur;
    [SerializeField] TMP_Text luckMax;
    [SerializeField] TMP_Text luckCost;
    public static int pLuckLevel = 0;
    static int pLuckMax = 3;
    [SerializeField] int luckCostAmount;
    [SerializeField] int luckLevelStat = 1;

    [Header("----- RevivesP ------")]
    [SerializeField] TMP_Text revivesCur;
    [SerializeField] TMP_Text revivesMax;
    [SerializeField] TMP_Text revivesCost;
    public static int pRevivesLevel = 0;
    static int pRevivesMax = 1;
    [SerializeField] int revivesCostAmount;
    [SerializeField] int revivesLevelStat = 1;

    [Header("----- RerollP ------")]
    [SerializeField] TMP_Text rerollCur;
    [SerializeField] TMP_Text rerollMax;
    [SerializeField] TMP_Text rerollCost;
    public static int pRerollLevel = 0;
    static int pRerollMax = 3;
    [SerializeField] int rerollCostAmount;
    [SerializeField] int rerollLevelStat = 1;

    static bool firstTime = true;
    public bool dataDeleted = false;

    public static int healthP = 0;
    public static float speedP = 0;
    public static int damageP = 0;
    public static int armorP = 0;
    public static float castSpeedP = 0;
    public static int luckP = 0;
    public static int revivesP = 0;
    public static int rerollP = 0;


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
            healthP = PlayerPrefs.GetInt("Health", 0);
            speedP = PlayerPrefs.GetFloat("Speed", 0);
            damageP = PlayerPrefs.GetInt("Damage", 0);
            luckP = PlayerPrefs.GetInt("Luck", 0);
            armorP = PlayerPrefs.GetInt("Armor", 0);
            castSpeedP = PlayerPrefs.GetFloat("CastSpeed", 0);
            revivesP = PlayerPrefs.GetInt("Revives", 0);
            rerollP = PlayerPrefs.GetInt("Rerolls", 0);

            pHealthLevel = PlayerPrefs.GetInt("HealthL", 0);
            pSpeedLevel = PlayerPrefs.GetInt("SpeedL", 0);
            pDamageLevel = PlayerPrefs.GetInt("DamageL", 0);
            pLuckLevel = PlayerPrefs.GetInt("LuckL", 0);
            pArmorLevel = PlayerPrefs.GetInt("ArmorL", 0);
            pCastSpeedLevel = PlayerPrefs.GetInt("CastSpeedL", 0);
            pRevivesLevel = PlayerPrefs.GetInt("RevivesL", 0);
            pRerollLevel = PlayerPrefs.GetInt("RerollsL", 0);

            menuActive = startScreen;
            firstTime = false;
        }
        else
        {
            menuActive = null;
        }

        updateShop();
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
        healthCost.text = healthCostAmount.ToString();

        damageCur.text = pDamageLevel.ToString();
        damageMax.text = pDamageMax.ToString();
        damageCost.text = damageCostAmount.ToString();

        speedCur.text = pSpeedLevel.ToString();
        speedMax.text = pSpeedMax.ToString();
        speedCost.text = speedCostAmount.ToString();

        armorCur.text = pArmorLevel.ToString();
        armorMax.text = pArmorMax.ToString();
        armorCost.text = armorCostAmount.ToString();

        luckCur.text = pLuckLevel.ToString();
        luckMax.text = pLuckMax.ToString();
        luckCost.text = luckCostAmount.ToString();

        castSpeedCur.text = pCastSpeedLevel.ToString();
        castSpeedMax.text = pCastSpeedMax.ToString();
        castSpeedCost.text = castSpeedCostAmount.ToString();

        revivesCur.text = pRevivesLevel.ToString();
        revivesMax.text = pRevivesMax.ToString();
        revivesCost.text = revivesCostAmount.ToString();

        rerollCur.text = pRerollLevel.ToString();
        rerollMax.text = pRerollMax.ToString();
        rerollCost.text = rerollCostAmount.ToString();

        goldCur.text = Gamemanager.gold.ToString();
    }

    public void pHealthUp()
    {
        if (pHealthLevel < pHealthMax && (Gamemanager.gold >= healthCostAmount))
        {
            Gamemanager.gold = Gamemanager.gold - healthCostAmount;
            pHealthLevel++;
            healthP = healthP + healthLevelStat;
            updateShop();
        }
    }
    public void pSpeedUp()
    {
        if (pSpeedLevel < pSpeedMax && (Gamemanager.gold >= speedCostAmount))
        {
            Gamemanager.gold = Gamemanager.gold - speedCostAmount;
            pSpeedLevel++;
            speedP = speedP + speedLevelStat;
            updateShop();
        }
    }

    public void pDamageUp()
    {
        if (pDamageLevel < pDamageMax && (Gamemanager.gold >= damageCostAmount))
        {
            Gamemanager.gold = Gamemanager.gold - damageCostAmount;
            pDamageLevel++;
            damageP = damageP + damageLevelStat;
            updateShop();
        }
    }

    public void pArmorUp()
    {
        if (pArmorLevel < pArmorMax && (Gamemanager.gold >= armorCostAmount))
        {
            Gamemanager.gold = Gamemanager.gold - armorCostAmount;
            pArmorLevel++;
            armorP = armorP + armorLevelStat;
            updateShop();
        }
    }

    public void pCastSpeedUp()
    {
        if (pCastSpeedLevel < pCastSpeedMax && (Gamemanager.gold >= castSpeedCostAmount))
        {
            Gamemanager.gold = Gamemanager.gold - castSpeedCostAmount;
            pCastSpeedLevel++;
            castSpeedP = castSpeedP + castSpeedLevelStat;
            updateShop();
        }
    }

    public void pLuckUp()
    {
        if (pLuckLevel < pLuckMax && (Gamemanager.gold >= luckCostAmount))
        {
            Gamemanager.gold = Gamemanager.gold - luckCostAmount;
            pLuckLevel++;
            luckP = luckP + luckLevelStat;
            updateShop();
        }
    }

    public void pRevivesUp()
    {
        if (pRevivesLevel < pRevivesMax && (Gamemanager.gold >= revivesCostAmount))
        {
            Gamemanager.gold = Gamemanager.gold - revivesCostAmount;
            pRevivesLevel++;
            revivesP = revivesP + revivesLevelStat;
            updateShop();
        }
    }

    public void pRerollsUp()
    {
        if (pRerollLevel < pRerollMax && (Gamemanager.gold >= rerollCostAmount))
        {
            Gamemanager.gold = Gamemanager.gold - rerollCostAmount;
            pRerollLevel++;
            rerollP = rerollP + rerollLevelStat;
            updateShop();
        }
    }
}
