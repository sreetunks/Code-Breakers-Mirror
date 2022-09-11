using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] public AudioSource musicSource, effectSource;
    [SerializeField] private Slider masterSlider, musicSlider, effectSlider;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        LoadSettings();
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;

        if(musicSource.mute == true)
        {
            musicSource.Stop();
        }
        else
        {
            musicSource.Play();
        }
    }

    public void ToggleEffects()
    {
        effectSource.mute = !effectSource.mute;

        if (effectSource.mute == true)
        {
            effectSource.Stop();
        }
        else
        {
            effectSource.Play();
        }
    }

    public void ChangeMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);

        PlayerPrefs.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXrVolume", Mathf.Log10(volume) * 20);

        musicSlider.value = volume;
        effectSlider.value = volume;
    }

    public void ChangeMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }

    public void ChangeSfxVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
            audioMixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("MasterVolume"));
        }


        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
            audioMixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("MusicVolume"));
        }

        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            effectSlider.value = PlayerPrefs.GetFloat("SFXVolume");
            audioMixer.SetFloat("SFXVolume", PlayerPrefs.GetFloat("SFXVolume"));
        }
    }
}