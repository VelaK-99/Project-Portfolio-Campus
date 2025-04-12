using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class audioSettings : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioMixer audioMixer;
    public Slider volumeSlider;

    void Start()
    {
       
        if (PlayerPrefs.HasKey("volume"))
        {
            float savedVolume = PlayerPrefs.GetFloat("volume");
            volumeSlider.value = savedVolume;
            SetVolume(savedVolume);
        }

        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float volume)
    {
       
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);

        
        PlayerPrefs.SetFloat("volume", volume);
    }
}
