using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] public AudioSource musicSource, effectSource;
    [SerializeField] private Slider masterSlider, musicSlider, effectSlider;

    private void Start()
    {
        LoadSettings();

        masterSlider.onValueChanged.AddListener(ChangeMasterVolume);
        musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
        effectSlider.onValueChanged.AddListener(ChangeSfxVolume);
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
        float masterVolume = Mathf.Log10(volume) * 20;
        audioMixer.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void ChangeMusicVolume(float volume)
    {
        float musicVolume = Mathf.Log10(volume) * 20;
        audioMixer.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void ChangeSfxVolume(float volume)
    {
        float effectsVolume = Mathf.Log10(volume) * 20;
        audioMixer.SetFloat("SFXVolume", effectsVolume);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            float masterVolume = PlayerPrefs.GetFloat("MasterVolume");
            masterSlider.value = masterVolume;
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20);
        }


        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            float musicVolume = PlayerPrefs.GetFloat("MusicVolume");
            musicSlider.value = musicVolume;
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20);
        }

        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            float effectsVolume = PlayerPrefs.GetFloat("SFXVolume");
            effectSlider.value = effectsVolume;
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(effectsVolume) * 20);
        }
    }
}