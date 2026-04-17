using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class buttonFunctions : MonoBehaviour
{
    public string startButtonScene;
    string menuScene = "MenuScene";
    [SerializeField] GameObject levelUpButton;

    // Type 0 = health | 1 = speed | 2 = damage
    int levelUpType;

    // Tier 0 = common | Tier 1 = rare | Tier 2 = epic
    int levelUpTier;

    public void resume()
    {
        Gamemanager.instance.stateUnpause();
    }

    public void restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void healthUp()
    {
        Gamemanager.instance.playerScript.hpLevelUp();
        Gamemanager.instance.playerScript.updatePlayerUI();
        Gamemanager.instance.stateUnpause();
    }

    public void damageUp()
    {
        Gamemanager.instance.playerScript.damageLevelUp();
        Gamemanager.instance.stateUnpause();
    }

    public void speedUp()
    {
        Gamemanager.instance.playerScript.speedLevelUp();
        Gamemanager.instance.stateUnpause();
    }

    public void levelUp()
    {
        if (levelUpType == 0)
        {
            if (levelUpTier == 0)
            {
                Gamemanager.instance.playerScript.hpLevelUp();
                Gamemanager.instance.playerScript.updatePlayerUI();
                Gamemanager.instance.stateUnpause();
            }
        }

        if (levelUpType == 1)
        {
            Gamemanager.instance.playerScript.speedLevelUp();
            Gamemanager.instance.stateUnpause();
        }

        if (levelUpType == 2)
        {
            Gamemanager.instance.playerScript.damageLevelUp();
            Gamemanager.instance.stateUnpause();
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