using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [SerializeField] Animator doorAnim;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            doorAnim.SetTrigger("Open");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            doorAnim.SetTrigger("Closed");
        }
    }
}
