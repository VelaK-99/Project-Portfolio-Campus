using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource;
    public Slider volumeSlider;

    void Start()
    {
       
        volumeSlider.value = audioSource.volume;

        
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }
}
