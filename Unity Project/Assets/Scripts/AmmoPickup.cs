using UnityEngine;

public class AmmoPickup : MonoBehaviour
{

    [SerializeField] int ammoAmt;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerScript player = other.GetComponent<playerScript>();
            
            if(player != null )
            {
                player.AddAmmo(ammoAmt);
                Destroy(gameObject);
            }
        }
    }
}
