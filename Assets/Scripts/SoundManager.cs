using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource _musicSource, _effectSource;
    [SerializeField] private Slider _masterSlider, _musicSlider, _effectSlider; 

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        _musicSlider.value = _masterSlider.value;
        _effectSlider.value = _masterSlider.value;
    }

    public void ToggleMusic()
    {
        _musicSource.mute = !_musicSource.mute;
    }

    public void ToggleEffects()
    {
        _effectSource.mute = !_effectSource.mute;
    }

    public void ChangeMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);

        _musicSource.volume = volume;
        _effectSource.volume = volume;

        _musicSlider.value = volume;
        _effectSlider.value = volume;
    }

    public void ChangeMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        _musicSource.volume = volume;
    }

    public void ChangeSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        _effectSource.volume = volume;
    }
}