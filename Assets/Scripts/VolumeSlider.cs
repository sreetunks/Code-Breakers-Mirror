using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private Slider _slider;

    void Start()
    {
        _slider.value = 0.5f;

        SoundManager.Instance.ChangeMasterVolume(_slider.value);
        SoundManager.Instance.ChangeMusicVolume(_slider.value);
        SoundManager.Instance.ChangeSFXVolume(_slider.value);

        if (_slider = GameObject.FindGameObjectWithTag("MasterSlider").GetComponent<Slider>())
        {
            _slider.onValueChanged.AddListener(val => SoundManager.Instance.ChangeMasterVolume(val));
            _slider.onValueChanged.AddListener(val => SoundManager.Instance.ChangeMusicVolume(val));
            _slider.onValueChanged.AddListener(val => SoundManager.Instance.ChangeSFXVolume(val));
        }

        if (_slider = GameObject.FindGameObjectWithTag("MusicSlider").GetComponent<Slider>())
        {
            _slider.onValueChanged.AddListener(val => SoundManager.Instance.ChangeMusicVolume(val));
        }
            
        if (_slider = GameObject.FindGameObjectWithTag("SFXSlider").GetComponent<Slider>())
        {
            _slider.onValueChanged.AddListener(val => SoundManager.Instance.ChangeSFXVolume(val));
        }
    }
}