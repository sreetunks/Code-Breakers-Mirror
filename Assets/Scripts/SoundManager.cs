using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] public AudioSource _musicSource, _effectSource;
    [SerializeField] private Slider _masterSlider, _musicSlider, _effectSlider; 

    void Awake()
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
        _musicSource.mute = !_musicSource.mute;

        if(_musicSource.mute == true)
        {
            _musicSource.Stop();
        }
        else
        {
            _musicSource.Play();
        }
    }

    public void ToggleEffects()
    {
        _effectSource.mute = !_effectSource.mute;

        if (_effectSource.mute == true)
        {
            _effectSource.Stop();
        }
        else
        {
            _effectSource.Play();
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

        _musicSlider.value = volume;
        _effectSlider.value = volume;
    }

    public void ChangeMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }

    public void ChangeSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }

    public void LoadSettings()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            _masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
            audioMixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("MasterVolume"));
        }

        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            _musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
            audioMixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("MusicVolume"));
        }

        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            _effectSlider.value = PlayerPrefs.GetFloat("SFXVolume");
            audioMixer.SetFloat("SFXVolume", PlayerPrefs.GetFloat("SFXVolume"));
        }
    }
}