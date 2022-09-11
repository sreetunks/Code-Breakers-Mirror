using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;

    private void Start()
    {
        slider.value = 0.5f;

        SoundManager.Instance.ChangeMasterVolume(slider.value);
        SoundManager.Instance.ChangeMusicVolume(slider.value);
        SoundManager.Instance.ChangeSfxVolume(slider.value);

        if (slider == GameObject.FindGameObjectWithTag("MasterSlider").GetComponent<Slider>())
        {
            slider.onValueChanged.AddListener(val => SoundManager.Instance.ChangeMasterVolume(val));
            slider.onValueChanged.AddListener(val => SoundManager.Instance.ChangeMusicVolume(val));
            slider.onValueChanged.AddListener(val => SoundManager.Instance.ChangeSfxVolume(val));
        }

        if (slider == GameObject.FindGameObjectWithTag("MusicSlider").GetComponent<Slider>())
        {
            slider.onValueChanged.AddListener(val => SoundManager.Instance.ChangeMusicVolume(val));
        }
            
        if (slider == GameObject.FindGameObjectWithTag("SFXSlider").GetComponent<Slider>())
        {
            slider.onValueChanged.AddListener(val => SoundManager.Instance.ChangeSfxVolume(val));
        }
    }
}