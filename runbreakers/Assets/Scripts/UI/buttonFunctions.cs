using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class buttonFunctions : MonoBehaviour
{
    public string startButtonScene;
    string menuScene = "MenuScene";

    public void resume()
    {
        Gamemanager.instance.stateUnpause();
    }

    public void restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void startLevelScene()
    {
        SceneManager.LoadScene(startButtonScene);
    }

    public void startMenuScene()
    {
        SceneManager.LoadScene(menuScene);
    }

    public void startScreen()
    {
        mainMenuManager.instance.prevMenu = mainMenuManager.instance.menuActive;
        mainMenuManager.instance.menuActive = null;
    }

    public void settings()
    {
        mainMenuManager.instance.prevMenu = mainMenuManager.instance.menuActive;
        mainMenuManager.instance.menuActive = mainMenuManager.instance.settings;
    }

    public void credits()
    {
        mainMenuManager.instance.prevMenu = mainMenuManager.instance.menuActive;
        mainMenuManager.instance.menuActive = mainMenuManager.instance.credits;
    }

    public void perShop()
    {
        mainMenuManager.instance.prevMenu = mainMenuManager.instance.menuActive;
        mainMenuManager.instance.menuActive = mainMenuManager.instance.permanentShop;
    }

    public void rerollUpgrade()
    {
        if (Gamemanager.instance.isLevelUp == true)
        {
            if (Gamemanager.instance.rolling == false)
            {
                if (Gamemanager.instance.rerollChance < Gamemanager.instance.rerollLimit)
                {
                    Gamemanager.instance.isRerolled = true;
                    Gamemanager.instance.rerollChance++;
                    Gamemanager.instance.rolling = true;
                    Gamemanager.instance.rerollOn.SetActive(true);
                    Gamemanager.instance.rerollOff.SetActive(false);
                    Gamemanager.instance.updateRerollButton();
                }
            }
            else
            {
                Gamemanager.instance.rolling = false;
                Gamemanager.instance.rerollOn.SetActive(false);
                Gamemanager.instance.rerollOff.SetActive(true);
                Gamemanager.instance.updateRerollButton();
                Gamemanager.instance.isRerolled = false;
            }
        }
    }

    public void deleteData()
    {
        PlayerPrefs.DeleteAll();

        mainMenuManager.healthP = 0;
        mainMenuManager.speedP = 0;
        mainMenuManager.damageP = 0;
        mainMenuManager.armorP = 0;
        mainMenuManager.luckP = 0;
        mainMenuManager.castSpeedP = 0;
        mainMenuManager.reviveP = 0;
        mainMenuManager.rerollP = 0;

        Gamemanager.gold = 100;

        mainMenuManager.pHealthLevel = 0;
        mainMenuManager.pSpeedLevel = 0;
        mainMenuManager.pDamageLevel = 0;
        mainMenuManager.pLuckLevel = 0;
        mainMenuManager.pArmorLevel = 0;
        mainMenuManager.pCastSpeedLevel = 0;
        mainMenuManager.pReviveLevel = 0;
        mainMenuManager.pRerollLevel = 0;

        mainMenuManager.instance.dataDeleted = true;
    }

    public void quit()
    {
        PlayerPrefs.SetInt("Health", mainMenuManager.healthP);
        PlayerPrefs.SetFloat("Speed", mainMenuManager.speedP);
        PlayerPrefs.SetInt("Damage", mainMenuManager.damageP);
        PlayerPrefs.SetInt("Luck", mainMenuManager.luckP);
        PlayerPrefs.SetFloat("CastSpeed", mainMenuManager.castSpeedP);
        PlayerPrefs.SetInt("Armor", mainMenuManager.armorP);
        PlayerPrefs.SetInt("Revive", mainMenuManager.reviveP);
        PlayerPrefs.SetInt("Reroll", mainMenuManager.rerollP);

        PlayerPrefs.SetInt("Gold", Gamemanager.gold);

        PlayerPrefs.SetInt("HealthL", mainMenuManager.pHealthLevel);
        PlayerPrefs.SetInt("SpeedL", mainMenuManager.pSpeedLevel);
        PlayerPrefs.SetInt("DamageL", mainMenuManager.pDamageLevel);
        PlayerPrefs.SetInt("LuckL", mainMenuManager.pLuckLevel);
        PlayerPrefs.SetInt("CastSpeedL", mainMenuManager.pCastSpeedLevel);
        PlayerPrefs.SetInt("ArmorL", mainMenuManager.pArmorLevel);
        PlayerPrefs.SetInt("ReviveL", mainMenuManager.pReviveLevel);
        PlayerPrefs.SetInt("RerollL", mainMenuManager.pRerollLevel);

        PlayerPrefs.SetFloat("MasterVolume", mainMenuManager.masterVol);
        PlayerPrefs.SetFloat("SFX", mainMenuManager.sfxVol);
        PlayerPrefs.SetFloat("MusicVolume", mainMenuManager.musicVol);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}