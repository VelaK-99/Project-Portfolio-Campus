using UnityEngine;

public class SingleSlidingDoorLocked : MonoBehaviour, IInteract
{
    [SerializeField] Animator SingleDoorAnim;
    public void Interact()
    {
        if (gameManager.instance.interactUI.activeSelf == false)
        {
            gameManager.instance.InteractTextUpdate("Open");
            gameManager.instance.interactUI.SetActive(true);
        }

        if (Input.GetButtonDown("Interact"))
        {
            SingleDoorAnim.SetTrigger("Open");
        }
    }

}
