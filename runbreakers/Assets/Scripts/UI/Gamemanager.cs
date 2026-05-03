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
    [SerializeField] GameObject bossChallenge;
    [SerializeField] GameObject lowHealth;

    [SerializeField] TMP_Text gameGoalCountText;
    [SerializeField] TMP_Text hpText;
    [SerializeField] TMP_Text curSpellNameText;
    [SerializeField] TMP_Text curSpellLevelText;
    [SerializeField] TMP_Text waveCountText;
    [SerializeField] TMP_Text waveTransitionText;
    [SerializeField] TMP_Text waveTimerText;

    public TMP_Text pauseHpText;
    public TMP_Text pauseSpeedText;
    public TMP_Text pauseDamageText;
    public TMP_Text pauseCastSpeedText;
    public TMP_Text pauseLuckText;
    public TMP_Text pauseReviveText;
    public TMP_Text pauseRerollText;
    public TMP_Text pauseArmorText;

    [Header("----- Audio ------")]
    public AudioClip click;
    public AudioSource audioSource;

    [Header("----- LevelUp ------")]
    public bool isLevelUp = false;
    public int rerollChance = 0;
    int rerollOriginal;
    public int rerollLimit;
    public bool isRerolled = false;
    public bool rolling = false;
    [SerializeField] TMP_Text rerollCurText;
    [SerializeField] TMP_Text rerollLimitText;
    [SerializeField] TMP_Text rerollText;
    public GameObject rerollOff;
    public GameObject rerollOn;

    public int tier1Min = 0;
    public int tier1Max = 75;
    public int tier2Min = 76;
    public int tier2Max = 95;
    public int tier3Min = 96;
    public int tier3Max = 100;

    [Header("----- Boss UI ------")]
    [SerializeField] GameObject bossHPBar;
    [SerializeField] Image bossCurrentHPBar;
    [SerializeField] TMP_Text bossHPText;

    [Header("----- Mini Boss UI ------")]
    [SerializeField] GameObject miniBossHPBar;
    [SerializeField] Image miniBossCurrentHPBar;
    [SerializeField] TMP_Text miniBossHPText;

    [Header("----- Wave Transition ------")]
    [SerializeField] float waveTransitionTime = 2f;

    [Header("----- Cursor ------")]
    public Texture2D cursorTexture;
    public Vector2 hotSpot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    [Header("----- Player ------")]
    [SerializeField] TMP_Text levels;
    [SerializeField] TMP_Text sprintMsg;
    public Image playerHPBar;
    public Image playerXPBar;
    public Image speedBuffBar;
    public Image speedDebuffBar;
    public Image damageBuffBar;
    public Image dashCooldownImage;
    public GameObject damagePlayerFlash;
    public GameObject player;
    public playerControl playerScript;
    public bool isPaused;
    public bool canSummonBoss;
    public bool bossSummoned;
    public static int gold = 100;

    float timeScaleOrig;
    int gameGoalCount;
    int levelCur;

    void Awake()
    {
        instance = this;
        timeScaleOrig = 1f;

        canSummonBoss = true;
        bossSummoned = false;

        rerollOriginal = rerollChance + mainMenuManager.rerollP;
        rerollChance = rerollOriginal;
        rerollOn.SetActive(false);
        rerollOff.SetActive(true);

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerControl>();

        hotSpot.x = 32;
        hotSpot.y = 32;
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);

        isPaused = false;
        Time.timeScale = 1f;
        menuActive = null;

        if (menuPause != null) menuPause.SetActive(false);
        if (menuWin != null) menuWin.SetActive(false);
        if (menuLose != null) menuLose.SetActive(false);
        if (levelUpMenu != null) levelUpMenu.SetActive(false);
        if (bossHPBar != null) bossHPBar.SetActive(false);
        if (miniBossHPBar != null) miniBossHPBar.SetActive(false);
        if (dashCooldownImage != null) dashCooldownImage.gameObject.SetActive(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (waveTransitionText != null)
            waveTransitionText.gameObject.SetActive(false);

        if (waveTimerText != null)
            waveTimerText.text = "";
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel") || Input.GetButtonDown("Pause"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
                playerScript.updateStatDisplay();
            }
            else if (menuActive == menuPause)
            {
                stateUnpause();
            }
        }
        UpdateDashUI();
    }

    void UpdateDashUI()
    {
        if (dashCooldownImage != null && playerScript != null)
        {
            float fillAmount = playerScript.GetDashCooldownFill();
            dashCooldownImage.gameObject.SetActive(fillAmount > 0);
            dashCooldownImage.fillAmount = fillAmount;
        }
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
        sprintMsg.gameObject.SetActive(false);
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
        if (gameGoalCount < 0) gameGoalCount = 0;
        gameGoalCountText.text = gameGoalCount.ToString("F0");
    }

    public void setGameGoal(int amount)
    {
        gameGoalCount = amount;
        if (gameGoalCount < 0) gameGoalCount = 0;
        gameGoalCountText.text = gameGoalCount.ToString("F0");
    }

    public void setWaveCount(int currentWave, int totalWaves)
    {
        if (waveCountText != null)
            waveCountText.text = currentWave + "/" + totalWaves;
    }

    public void updateWaveTimer(float timeLeft)
    {
        if (waveTimerText == null) return;

        int secondsLeft = Mathf.CeilToInt(timeLeft);
        if (secondsLeft < 0) secondsLeft = 0;
        waveTimerText.text = "Time: " + secondsLeft;
    }

    public void showWaveTransition(int waveNum)
    {
        StartCoroutine(waveTransition(waveNum));
    }

    IEnumerator waveTransition(int waveNum)
    {
        if (waveTransitionText == null) yield break;

        waveTransitionText.gameObject.SetActive(true);
        waveTransitionText.text = "Wave " + waveNum;
        yield return new WaitForSeconds(waveTransitionTime);
        waveTransitionText.gameObject.SetActive(false);
    }

    public void setBossText()
    {
        if (waveCountText != null)
            waveCountText.text = "BOSS WAVE";

        backgroundMusic.instance.PlayBossMusic();

        if (waveTimerText != null)
            waveTimerText.text = "";
    }

    public void showBossChallenge()
    {
        bossChallenge.SetActive(true);
    }

    public void hideBossChallenge()
    {
        bossChallenge.SetActive(true);
    }

    public void destroyAllEnemies()
    {
        GameObject[] objectsToDestroy = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject obj in objectsToDestroy)
            Destroy(obj);
    }

    #region LevelUpFunctions

    public void LevelUp()
    {
        levelCur += 1;
        isLevelUp = true;
        rerollChance = 0;
        updateRerollButton();
        menuActive = levelUpMenu;
        menuActive.SetActive(true);

        if (playerScript.GetCurrentLevel() == 3)
            sprintMsg.gameObject.SetActive(true);

        statePause();
        setLevelText();
    }

    public void AddGold(int amount)
    {
        gold += amount;
        if (GoldUI.instance != null)
            GoldUI.instance.UpdateGold(gold);
    }

    public void setLevelText()
    {
        levels.text = levelCur.ToString("F0");
    }

    public void updateRerollButton()
    {
        if (rolling == false)
        {
            rerollText.text = "Rerolls:";
            rerollCurText.text = rerollChance.ToString();
            rerollLimitText.text = rerollLimit.ToString();
        }
        else
        {
            rerollText.text = "Stop";
            rerollCurText.text = "";
            rerollLimitText.text = "Rolling";
        }
    }
    #endregion

    public void lowHealthOn()
    {
        lowHealth.gameObject.SetActive(true);
    }

    public void lowHealthOff()
    {
        lowHealth.gameObject.SetActive(false);
    }

    public void onClick()
    {
        if (click != null)
        {
            audioSource.clip = click;
            audioSource.loop = false;
            audioSource.Play();
        }
    }

    public void showWin()
    {
        statePause();
        menuActive = menuWin;
        menuActive.SetActive(true);
    }

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public GameObject GetBossHPBar()
    {
        return bossHPBar;
    }
    public Image GetBossCurrentHPBar() 
    {
        return bossCurrentHPBar;
    }
    public TMP_Text GetBossHPText()
    {
        return bossHPText;
    }

    public GameObject GetMiniBossHPBar()
    {
        return miniBossHPBar;
    }
    public Image GetMiniBossCurrentHPBar()
    {
        return miniBossCurrentHPBar;
    }
    public TMP_Text GetMiniBossHPText()
    {
        return miniBossHPText;
    }

    public void setSpellNameText(string name)
    {
        if (curSpellNameText != null)
            curSpellNameText.text = name;
    }

    public void setSpellLevelText(int level)
    {
       if (curSpellLevelText != null)
        {
            curSpellLevelText.text = "LV." + level;
        }
    }
}