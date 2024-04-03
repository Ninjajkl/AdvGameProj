using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MixerController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    public void SetVolume(string groupName, float sliderValue)
    {
        audioMixer.SetFloat(groupName, Mathf.Log10(sliderValue) * 20);
    }

    public void SetMasterVolume(float sliderValue) { SetVolume("MasterVolume", sliderValue); }
    public void SetMusicVolume(float sliderValue) { SetVolume("MusicVolume", sliderValue); }
    public void SetSoundEffectsVolume(float sliderValue) { SetVolume("SoundEffectsVolume", sliderValue); }
    public void SetUIVolume(float sliderValue) { SetVolume("UIVolume", sliderValue); }
}
