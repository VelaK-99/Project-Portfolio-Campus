using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

public class buttonFunctions : MonoBehaviour
{
    [SerializeField] private GameObject firstButtonMenu;
    [SerializeField] private GameObject firstButtonSet;
    public AudioSource audioSource;
    [SerializeField] string winRestartButton;




    public void ClickSound()
    {
        if (audioSource != null)
            audioSource.Play();
    }
    public void resume()
    {
        
        gameManager.instance.stateUnpause();

    }

    public void restart()
    {
       
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateUnpause();
        ClickSound();
    }

    public void WinRestart()
    {
        SceneManager.LoadScene(winRestartButton);
        gameManager.instance.stateUnpause();
        ClickSound();
    }
    public void respawn()
    {
       
        gameManager.instance.playerScript.spawnPlayer();
        gameManager.instance.stateUnpause();
    }
    public void OpenSetting()
    {
        ClickSound();
        gameManager.instance.painelSettings.SetActive(true);
        gameManager.instance.hudPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(firstButtonSet);
    }
    public void CloseSetting()
    {
        ClickSound();
        gameManager.instance.painelSettings.SetActive(false);
        gameManager.instance.hudPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstButtonMenu);
    }
    public void Quit()
    {
        ClickSound();
        PlayerPrefs.DeleteAll();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
Application.Quit();
#endif
    }

    public void CreditsScene(string sceneMenu)
    {
        SceneManager.LoadScene(sceneMenu);
    }
}
