using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [SerializeField] Animator doorAnim;
    private bool locked = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !locked)
        {
            doorAnim.SetTrigger("Open");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player") && !locked)
        {
            doorAnim.SetTrigger("Closed");
        }
    }

    public void LockDoor()
    {
    locked = true;
    }

    public void UnlockDoor()
    {
        locked = false;
    }
}
