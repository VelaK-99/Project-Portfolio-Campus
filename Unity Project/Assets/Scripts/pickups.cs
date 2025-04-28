using Unity.Hierarchy;
using UnityEngine;

public class pickups : MonoBehaviour
{
    [SerializeField] gunStats gun;
    enum pickupType { healthPack, ammoPack, gun }
    [SerializeField] int healthAmount;
    [SerializeField] int ammoAmount;
    [SerializeField] pickupType type;
    [SerializeField] float destroyTime;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (type == pickupType.healthPack || type == pickupType.ammoPack)
        {
            Destroy(gameObject, destroyTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IPickup pickupable = other.GetComponent<IPickup>();

        if (pickupable != null && type == pickupType.healthPack)
        {
            if (gameManager.instance.playerScript.getOrigHP() > gameManager.instance.playerScript.getCurHP())
            {
                pickupable.HealthPickup(healthAmount);
                Destroy(gameObject);
            }
        }

        if (pickupable != null && type == pickupType.ammoPack)
        {
            pickupable.AmmoPickup(ammoAmount);
            Destroy(gameObject);
        }

        if (pickupable != null && type == pickupType.gun)
        {
            pickupable.GetGunStats(gun);
            Destroy(gameObject);
        }

    }
}