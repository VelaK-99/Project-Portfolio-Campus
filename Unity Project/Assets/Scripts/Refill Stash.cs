using UnityEngine;

public class RefillStash : MonoBehaviour, IInteract
{
    [SerializeField] Animator stashAnim;

    // Interact method uses the gameManager to change the HP, ammo, or whatever else is needed.
    public void Interact()
    {
        if(gameManager.instance.interactUI.activeSelf == false)
        {
            gameManager.instance.InteractTextUpdate("Refill");
            gameManager.instance.interactUI.SetActive(true);
        }

        if(Input.GetButtonDown("Interact")) // if the player interacts with the object using the E key it does this function
        {
            gameManager.instance.playerScript.HealthPickup(gameManager.instance.playerScript.getOrigHP());
            gameManager.instance.playerScript.RefillAllAmmo();

        }
    }
}
