using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private GameObject painelMenu;
    [SerializeField] private GameObject painelSettings;
    public AudioSource audioSource;



    void Start()
    {
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;
    }

    public void ClickSound()
    {
        if (audioSource != null)
            audioSource.Play();
    }


    public void Play()
    {
        ClickSound();
        SceneManager.LoadScene(sceneName);
    }

    public void OpenSetting()
    {
        ClickSound();
        painelMenu.SetActive(false);
        painelSettings.SetActive(true);
    }

    public void CloseSetting()
    {
        ClickSound();
        painelSettings.SetActive(false);
        painelMenu.SetActive(true);
    }

    public void Quit()
    {
        ClickSound();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
Application.Quit();
#endif
    }
}
