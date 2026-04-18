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
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}