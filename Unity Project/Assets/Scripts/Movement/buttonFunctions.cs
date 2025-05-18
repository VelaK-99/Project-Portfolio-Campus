using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class buttonFunctions : MonoBehaviour
{
    
    public void resume()
    {
        gameManager.instance.stateUnpause();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateUnpause();
    }

    public void respawn()
    {
        gameManager.instance.playerScript.spawnPlayer();
        gameManager.instance.stateUnpause();
    }
    public void OpenSetting()
    {
       
        gameManager.instance.painelSettings.SetActive(true);
    }
    public void CloseSetting()
    {

        gameManager.instance.painelSettings.SetActive(false);
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
