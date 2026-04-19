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
    List<GameObject> menus = new List<GameObject>();
    public GameObject prevMenu;
    int pHealthLevel = 0;
    int pSpeedLevel = 0;
    int pDamageLevel = 0;
    
    void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        menuActive = startScreen;
    }

    // Update is called once per frame
    void Update()
    {
        if (menuActive == null && prevMenu != null)
        {
            prevMenu.SetActive(false);
        }
        else
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
    }

    public void changeMenu()
    {

    }
}
