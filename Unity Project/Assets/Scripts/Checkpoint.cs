using System.Collections;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] Renderer model;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && gameManager.instance.playerSpawnPos.transform.position != transform.position)
        {
            gameManager.instance.playerSpawnPos.transform.position = transform.position;
            StartCoroutine(checkpointFeedback());
        }
    }

    IEnumerator checkpointFeedback()
    {
        model.material.color = Color.red;
        gameManager.instance.checkpointPopup.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        gameManager.instance.checkpointPopup.SetActive(false);
        model.material.color = Color.white;
    }
}
