using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.ComponentModel;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager instance;

    [Header("----- Menus ------")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    [SerializeField] TMP_Text gameGoalCountText;
    [SerializeField] TMP_Text ammoCurText;
    [SerializeField] TMP_Text ammoMaxText;
    [SerializeField] TMP_Text waveCountText;
    [SerializeField] TMP_Text waveTransitionText;

    [Header("----- Wave Transition ------")]
    [SerializeField] float waveTransitionTime = 2f;

    [Header("----- Cursor ------")]
    public Texture2D cursorTexture;
    public Vector2 hotSpot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    [Header("----- Ammo ------")]
    [SerializeField] int ammoCur;
    [SerializeField] int ammoMax;

    [Header("----- Player ------")]
    public TMP_Text curLevel;
    public Image playerHPBar;
    public Image playerXPBar;
    public Image speedBuffBar;
    public Image speedDebuffBar;
    public Image damageBuffBar;
    public GameObject damagePlayerFlash;
    public GameObject player;
    public playerControl playerScript;
    public bool isPaused;

    float timeScaleOrig;
    int gameGoalCount;
    int level;

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

        if (waveTransitionText != null)
        {
            waveTransitionText.gameObject.SetActive(false);
        }
    }

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

        if (gameGoalCount < 0)
        {
            gameGoalCount = 0;
        }

        gameGoalCountText.text = gameGoalCount.ToString("F0");
    }

    public void setGameGoal(int amount)
    {
        gameGoalCount = amount;

        if (gameGoalCount < 0)
        {
            gameGoalCount = 0;
        }

        gameGoalCountText.text = gameGoalCount.ToString("F0");
    }

    public void setWaveCount(int currentWave, int totalWaves)
    {
        if (waveCountText != null)
        {
            waveCountText.text = currentWave + "/" + totalWaves;
        }
    }

    public void showWaveTransition(int waveNum)
    {
        StartCoroutine(waveTransition(waveNum));
    }

    IEnumerator waveTransition(int waveNum)
    {
        if (waveTransitionText == null)
            yield break;

        waveTransitionText.gameObject.SetActive(true);
        waveTransitionText.text = "Wave " + waveNum;
        yield return new WaitForSeconds(waveTransitionTime);
        waveTransitionText.gameObject.SetActive(false);
    }

    public void setBossText()
    {
        if (waveCountText != null)
        {
            waveCountText.text = "BOSS WAVE";
        }
    }

    public void showWin()
    {
        statePause();
        menuActive = menuWin;
        menuActive.SetActive(true);
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

    public void LevelUpText()
    {
        if (curLevel != null)
        {
            curLevel.text = level.ToString("F0");
        }
    }

    public void LevelUp()
    {
        level += 1;
        LevelUpText();
    }
}