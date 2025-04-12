using Unity.Hierarchy;
using UnityEngine;

public class pickups : MonoBehaviour
{
    enum pickupType {healthPack,ammoPack }
    [SerializeField] int healthAmount;
    [SerializeField] int ammoAmount;
    [SerializeField] pickupType type;
    [SerializeField] float destroyTime;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       if(type == pickupType.healthPack || type == pickupType.ammoPack)
        {
            Destroy(gameObject, destroyTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        IPickup pickup = other.GetComponent<IPickup>();

        if(pickup != null && type == pickupType.healthPack)
        {
            pickup.pickupHealth(healthAmount);
            Destroy(gameObject);
        }

        if(pickup != null && type == pickupType.ammoPack)
        {
            pickup.pickupAmmo(ammoAmount);
            Destroy(gameObject);
        }
    }
}
