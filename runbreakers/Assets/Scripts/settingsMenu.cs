using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class settingsMenu : MonoBehaviour
{
    [Header("---- Audio Mixer ----")]
    [SerializeField] AudioMixer mainMixer;

    [Header("---- Sliders ----")]
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    void Start()
    {
        if (masterSlider != null)
        {
            masterSlider.onValueChanged.AddListener(SetMasterVolume);
            SetMasterVolume(masterSlider.value);
        }

        if (musicSlider != null)
        {
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
            SetMusicVolume(musicSlider.value);
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
            SetSFXVolume(sfxSlider.value);
        }
    }

    public void SetMasterVolume(float value)
    {
        if (mainMixer != null)
        {
            mainMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20f);
        }
    }

    public void SetMusicVolume(float value)
    {
        if (mainMixer != null)
        {
            mainMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20f);
        }
    }

    public void SetSFXVolume(float value)
    {
        if (mainMixer != null)
        {
            mainMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20f);
        }
    }
}
