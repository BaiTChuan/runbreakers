using UnityEngine;

public class backgroundMusic : MonoBehaviour
{
    public static backgroundMusic instance;

    public AudioClip normalMusic;
    public AudioClip bossMusic;

    private AudioSource audioSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        PlayNormalMusic();
    }

    public void PlayNormalMusic()
    {
        if (normalMusic != null && audioSource.clip != normalMusic)
        {
            audioSource.clip = normalMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public void PlayBossMusic()
    {
        if (bossMusic != null && audioSource.clip != bossMusic)
        {
            audioSource.clip = bossMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}