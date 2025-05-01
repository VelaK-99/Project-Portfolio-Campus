using UnityEngine;
using UnityEngine.SceneManagement;

public class Victory : MonoBehaviour
{
    // Altere esse nome para o nome exato da sua cena de vitória
    public string winScene;

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se foi o jogador que entrou na área
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(winScene);
        }
    }
}
