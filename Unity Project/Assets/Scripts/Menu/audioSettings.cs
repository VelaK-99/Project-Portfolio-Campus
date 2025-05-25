using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AudioSettings : MonoBehaviour
{
    [Header("Audio Settings")]
    public List<AudioSource> audioSources; 
    public Slider volumeSlider;

    void Start()
    {
        if (audioSources != null && audioSources.Count > 0)
        {
            
            volumeSlider.value = audioSources[0].volume;
        }

        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float volume)
    {
        foreach (AudioSource source in audioSources)
        {
            if (source != null)
                source.volume = volume;
        }
    }
}
