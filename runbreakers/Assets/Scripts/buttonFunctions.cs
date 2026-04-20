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

    public void deleteData()
    {
        PlayerPrefs.DeleteAll();

        mainMenuManager.healthP = 0;
        mainMenuManager.speedP = 0;
        mainMenuManager.damageP = 0;
        mainMenuManager.luckP = 0;
        mainMenuManager.armorP = 0;
        mainMenuManager.castSpeedP = 0;
        mainMenuManager.revivesP = 0;
        mainMenuManager.rerollP = 0;

        mainMenuManager.pHealthLevel = 0;
        mainMenuManager.pSpeedLevel = 0;
        mainMenuManager.pDamageLevel = 0;
        mainMenuManager.pLuckLevel = 0;
        mainMenuManager.pArmorLevel = 0;
        mainMenuManager.pCastSpeedLevel = 0;
        mainMenuManager.pRevivesLevel = 0;
        mainMenuManager.pRerollLevel = 0;

        Gamemanager.gold = 100;

        mainMenuManager.instance.dataDeleted = true;
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
                    Gamemanager.instance.updateRerollButton();
                }
            }
            else
            {
                Gamemanager.instance.rolling = false;
                Gamemanager.instance.updateRerollButton();
                Gamemanager.instance.isRerolled = false;
            }
        }
    }

    public void quit()
    {
        PlayerPrefs.SetInt("Health", mainMenuManager.healthP);
        PlayerPrefs.SetFloat("Speed", mainMenuManager.speedP);
        PlayerPrefs.SetInt("Damage", mainMenuManager.damageP);
        PlayerPrefs.SetInt("Luck", mainMenuManager.luckP);
        PlayerPrefs.SetInt("Armor", mainMenuManager.armorP);
        PlayerPrefs.SetFloat("CastSpeed", mainMenuManager.castSpeedP);
        PlayerPrefs.SetInt("Revives", mainMenuManager.revivesP);
        PlayerPrefs.SetInt("Rerolls", mainMenuManager.rerollP);

        PlayerPrefs.SetInt("Gold", Gamemanager.gold);

        PlayerPrefs.SetInt("HealthL", mainMenuManager.pHealthLevel);
        PlayerPrefs.SetInt("SpeedL", mainMenuManager.pSpeedLevel);
        PlayerPrefs.SetInt("DamageL", mainMenuManager.pDamageLevel);
        PlayerPrefs.SetInt("LuckL", mainMenuManager.pLuckLevel);
        PlayerPrefs.SetInt("ArmorL", mainMenuManager.pArmorLevel);
        PlayerPrefs.SetInt("CastSpeedL", mainMenuManager.pCastSpeedLevel);
        PlayerPrefs.SetInt("RevivesL", mainMenuManager.pRevivesLevel);
        PlayerPrefs.SetInt("RerollsL", mainMenuManager.pRerollLevel);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}