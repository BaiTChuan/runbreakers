using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class buttonFunctions : MonoBehaviour
{

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

    public void quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}