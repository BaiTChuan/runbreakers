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
    [SerializeField] GameObject levelUpMenu;

    [SerializeField] TMP_Text gameGoalCountText;
    [SerializeField] TMP_Text hpText;
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

    [Header("----- Player ------")]
    [SerializeField] TMP_Text levels;
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
    int levelCur;

    void Awake()
    {
        instance = this;
        timeScaleOrig = 1f;

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerControl>();

        updateAmmoCurCount(Gamemanager.instance.playerScript.ammoCur);
        updateAmmoMaxCount(Gamemanager.instance.playerScript.ammoMax);
        hotSpot.x = 32;
        hotSpot.y = 32;
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);

        isPaused = false;
        Time.timeScale = 1f;
        menuActive = null;

        if (menuPause != null)
            menuPause.SetActive(false);

        if (menuWin != null)
            menuWin.SetActive(false);

        if (menuLose != null)
            menuLose.SetActive(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

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
        updateAmmoCurCount(Gamemanager.instance.playerScript.ammoCur);
        updateAmmoMaxCount(Gamemanager.instance.playerScript.ammoMax);
    }

    public void updateHpText(int hp)
    {
        hpText.text = hp.ToString("F0");
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

    public void LevelUp()
    {
        levelCur += 1;
        menuActive = levelUpMenu;
        menuActive.SetActive(true);
        statePause();
        setLevelText();
    }

    public void setLevelText()
    {
       levels.text = levelCur.ToString("F0");
    }

    public void showWin()
    {
        statePause();
        menuActive = menuWin;
        menuActive.SetActive(true);
    }

    public void updateAmmoCurCount(int amount)
    {
        ammoCurText.text = Gamemanager.instance.playerScript.ammoCur.ToString("F0");
    }

    public void updateAmmoMaxCount(int amount)
    {
        ammoMaxText.text = Gamemanager.instance.playerScript.ammoMax.ToString("F0");
    }

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
}