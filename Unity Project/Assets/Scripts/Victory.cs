using UnityEngine;
using UnityEngine.SceneManagement;

public class Victory : MonoBehaviour
{
    // Altere esse nome para o nome exato da sua cena de vit�ria
    public string winScene;

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se foi o jogador que entrou na �rea
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(winScene);
        }
    }
}
