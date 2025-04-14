using UnityEngine;

public class RefillStash : MonoBehaviour, IInteract
{

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
        gameManager.instance.playerScript.pickupHealth(gameManager.instance.playerScript.getOrigHP());
    }
}
