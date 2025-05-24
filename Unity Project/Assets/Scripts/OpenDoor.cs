using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [SerializeField] Animator doorAnim;
    [SerializeField] bool IsKeyPuzzle=false;
    private bool locked = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !locked )
        {
            if (!IsKeyPuzzle || (IsKeyPuzzle && gameManager.instance.haskey))
            {
                doorAnim.SetTrigger("Open");
            }
           
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player") && !locked)
        {
            if (!IsKeyPuzzle || (IsKeyPuzzle && gameManager.instance.haskey))
            {
                doorAnim.SetTrigger("Closed");
            }
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
