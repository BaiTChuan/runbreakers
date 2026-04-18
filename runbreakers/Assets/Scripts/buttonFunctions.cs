using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    public void levelUp()
    {
        if (Gamemanager.instance.levelUpType1 == 0)
        {
            if (levelUpTier == 0)
            {
                Gamemanager.instance.playerScript.hpLevelUp0();
                Gamemanager.instance.playerScript.updatePlayerUI();
                Gamemanager.instance.stateUnpause();
            }

            if (levelUpTier == 1)
            {
                Gamemanager.instance.playerScript.hpLevelUp1();
                Gamemanager.instance.playerScript.updatePlayerUI();
                Gamemanager.instance.stateUnpause();
            }

            if (levelUpTier == 2)
            {
                Gamemanager.instance.playerScript.hpLevelUp2();
                Gamemanager.instance.playerScript.updatePlayerUI();
                Gamemanager.instance.stateUnpause();
            }
        }

        if (levelUpType == 1)
        {
            if (levelUpTier == 0)
            {
                Gamemanager.instance.playerScript.speedLevelUp0();
                Gamemanager.instance.stateUnpause();
            }

            if (levelUpTier == 1)
            {
                Gamemanager.instance.playerScript.speedLevelUp1();
                Gamemanager.instance.stateUnpause();
            }

            if (levelUpTier == 2)
            {
                Gamemanager.instance.playerScript.speedLevelUp2();
                Gamemanager.instance.stateUnpause();
            }
        }

        if (levelUpType == 2)
        {
            if (levelUpTier == 0)
            {
                Gamemanager.instance.playerScript.damageLevelUp0();
                Gamemanager.instance.stateUnpause();
            }

            if (levelUpTier == 1)
            {
                Gamemanager.instance.playerScript.damageLevelUp1();
                Gamemanager.instance.stateUnpause();
            }

            if (levelUpTier == 2)
            {
                Gamemanager.instance.playerScript.damageLevelUp2();
                Gamemanager.instance.stateUnpause();
            }
        }
    }

    public void startLevelScene()
    {
        SceneManager.LoadScene(startButtonScene);
    }

    public void startMenuScene()
    {
        SceneManager.LoadScene(menuScene);
    }

    public void quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}