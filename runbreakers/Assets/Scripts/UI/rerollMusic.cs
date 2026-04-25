using UnityEngine;

public class rerollMusic : MonoBehaviour
{
    public static rerollMusic instance;

    public AudioClip reroll;

    public AudioSource audioSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    public void playReroll()
    {
        if (reroll != null)
        {
            audioSource.clip = reroll;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public void stopReroll()
    {
        audioSource.clip = null;
        audioSource.Stop();
    }
}
