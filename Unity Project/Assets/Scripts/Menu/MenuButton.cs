using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private GameObject Start_button;
    [SerializeField] private GameObject painelMenu;
    [SerializeField] private GameObject painelSettings;



    void Start()
    {
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;
    }


    public void Play()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OpenSetting()
    {
        painelMenu.SetActive(false);
        painelSettings.SetActive(true);
    }

    public void CloseSetting()
    {
        painelSettings.SetActive(false);
        painelMenu.SetActive(true);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
Application.Quit();
#endif
    }
}
