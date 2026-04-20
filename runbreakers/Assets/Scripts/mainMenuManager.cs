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

    [Header("----- HealthP ------")]
    [SerializeField] TMP_Text healthCur;
    [SerializeField] TMP_Text healthMax;
    public static int pHealthLevel = 0;
    static int pHealthMax = 3;
    [SerializeField] int healthLevelStat = 4;

    [Header("----- DamageP ------")]
    [SerializeField] TMP_Text damageCur;
    [SerializeField] TMP_Text damageMax;
    public static int pDamageLevel = 0;
    static int pDamageMax = 3;
    [SerializeField] int damageLevelStat = 2;

    [Header("----- SpeedP ------")]
    [SerializeField] TMP_Text speedCur;
    [SerializeField] TMP_Text speedMax;
    public static int pSpeedLevel = 0;
    static int pSpeedMax = 3;
    [SerializeField] float speedevelStat = 1;

    static bool firstTime = true;
    public bool dataDeleted = false;

    int pArmorLevel = 0;
    int pLuckLevel = 0;
    int pCastSpeedLevel = 0;

    public static int healthP = 0;
    public static float speedP = 0;
    public static int damageP = 0;

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

            pHealthLevel = PlayerPrefs.GetInt("HealthL", 0);
            pSpeedLevel = PlayerPrefs.GetInt("SpeedL", 0);
            pDamageLevel = PlayerPrefs.GetInt("DamageL", 0);

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
        damageCur.text = pDamageLevel.ToString();
        damageMax.text = pDamageMax.ToString();
        speedCur.text = pSpeedLevel.ToString();
        speedMax.text = pSpeedMax.ToString();
    }

    public void pHealthUp()
    {
        if (pHealthLevel < pHealthMax)
        {
            pHealthLevel++;
            healthP = healthP + healthLevelStat;
            healthCur.text = pHealthLevel.ToString();
        }
    }
    public void pSpeedUp()
    {
        if (pSpeedLevel < pSpeedMax)
        {
            pSpeedLevel++;
            speedP = speedP + speedevelStat;
            speedCur.text = pSpeedLevel.ToString();
        }
    }

    public void pDamageUp()
    {
        if (pDamageLevel < pDamageMax)
        {
            pDamageLevel++;
            damageP = damageP + damageLevelStat;
            damageCur.text = pDamageLevel.ToString();
        }
    }
}
