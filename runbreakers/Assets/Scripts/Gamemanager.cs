using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.ComponentModel;

public class gamemanager : MonoBehaviour
{
    public static gamemanager instance;

    [Header("----- Menus ------")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    [SerializeField] TMP_Text gameGoalCountText;
    [SerializeField] TMP_Text ammoCurText;
    [SerializeField] TMP_Text ammoMaxText;

    [Header("----- Cursor ------")]
    public Texture2D cursorTexture;
    public Vector2 hotSpot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    [Header("----- Ammo ------")]
    [SerializeField] int ammoCur;
    [SerializeField] int ammoMax;

    [Header("----- Buffs ------")]
    [SerializeField] public int healthBuffTimer;
    [SerializeField] public int healthBuffAmount;
    [SerializeField] public int speedBuffAmount;
    [SerializeField] public int speedBuffTimer;
    [SerializeField] public int attackBuffTimer;
    [SerializeField] public int attackBuffAmount;

    [Header("----- Player ------")]
    public Image playerHPBar;
    public GameObject player;
    public playerControl playerScript;
    public bool isPaused;

    private float timeScaleOrig;

    private int gameGoalCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerControl>();

        updateAmmoCurCount(ammoCur);
        updateAmmoMaxCount(ammoMax);
        hotSpot.x = 64;
        hotSpot.y = 64;
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                stateUnpause();
            }
        }
    }

    public void statePause()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void stateUnpause()
    {
        isPaused = false;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void updateGameGoal(int amount)
    {
        gameGoalCount += amount;
        gameGoalCountText.text = gameGoalCount.ToString("F0");

        if (gameGoalCount <= 0)
        {
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }

    public void updateAmmoCurCount(int amount)
    {
        ammoCurText.text = ammoCur.ToString("F0");
    }
    public void updateAmmoMaxCount(int amount)
    {
        ammoMaxText.text = ammoMax.ToString("F0");
    }

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
}
