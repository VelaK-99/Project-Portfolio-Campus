using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private GameObject painelMenu;
    [SerializeField] private GameObject creditsMenu;
    [SerializeField] private GameObject painelSettings;
    [SerializeField] private GameObject firstButtonMenu;
    [SerializeField] private GameObject firstButtonSet;
    public AudioSource audioSource;



    void Start()
    {
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;

        if (firstButtonMenu != null)
        {
            EventSystem.current.SetSelectedGameObject(firstButtonMenu);
        }
    }

    public void ClickSound()
    {
        if (audioSource != null)
            audioSource.Play();
    }


    public void Play(string sceneName)
    {
        ClickSound();
        SceneManager.LoadScene(sceneName);
    }

    public void OpenSetting()
    {
        ClickSound();
        painelMenu.SetActive(false);
        painelSettings.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstButtonSet);
    }

    public void CloseSetting()
    {
        ClickSound();
        painelSettings.SetActive(false);
        painelMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstButtonMenu);
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
