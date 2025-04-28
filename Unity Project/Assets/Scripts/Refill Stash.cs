using UnityEngine;

public class RefillStash : MonoBehaviour, IInteract
{
    [SerializeField] GameObject refillText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Interact method uses the gameManager to change the HP, ammo, or whatever else is needed.
    public void Interact()
    {
        gameManager.instance.textActive = refillText; // Sets the gameManager current text to what the object is linked to.
        gameManager.instance.textActive.SetActive(true);
        if(Input.GetButtonDown("Interact")) // if the player interacts with the object using the E key it does this function
        {
            gameManager.instance.playerScript.HealthPickup(gameManager.instance.playerScript.getOrigHP());
            gameManager.instance.playerScript.AmmoPickup(gameManager.instance.playerScript.GetMaxAmmo());

        }
    }

}
