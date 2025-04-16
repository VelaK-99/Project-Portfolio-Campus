using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private GameObject painelMenu;
    [SerializeField] private GameObject painelSettings;


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
