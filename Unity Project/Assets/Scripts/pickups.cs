using Unity.Hierarchy;
using UnityEngine;

public class pickups : MonoBehaviour
{
    enum pickupType { healthPack, ammoPack, Pistol, AssaultRifle, Shotgun }
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

    // Update is called once per frame
    void Update()
    {

    }

    //    private void OnTriggerEnter(Collider other)
    //    {
    //        IPickup pickup = other.GetComponent<IPickup>();

    //        if(pickup != null && type == pickupType.healthPack)
    //        {
    //            if (gameManager.instance.playerScript.getOrigHP() > gameManager.instance.playerScript.getCurHP())
    //            {
    //                pickup.pickupHealth(healthAmount);
    //                Destroy(gameObject);
    //            }
    //        }

    //        if(pickup != null && type == pickupType.ammoPack)
    //        {
    //            pickup.pickupAmmo(ammoAmount);
    //            Destroy(gameObject);
    //        }
    //    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerScript player = other.GetComponent<PlayerScript>();

        if (other.CompareTag("Player"))
        {
            if (player != null && type == pickupType.healthPack)
            {
                if (gameManager.instance.playerScript.getOrigHP() > gameManager.instance.playerScript.getCurHP())
                {
                    player.AddHealth(healthAmount);
                    Destroy(gameObject);
                }
            }

            if (player != null && type == pickupType.ammoPack)
            {
                player.AddAmmo(ammoAmount);
                Destroy(gameObject);
            }

            if(player != null && type == pickupType.Pistol)
            {
                player.UpdateWeapon(1, 25, 0.5f, 1.2f, 7);
                Destroy(gameObject);
            }

            if(player != null && type == pickupType.AssaultRifle)
            {
                player.UpdateWeapon(1, 35, 0.1f, 2.5f, 30);
                Destroy(gameObject);
            }

            if (player != null && type == pickupType.Shotgun)
            {
                player.UpdateWeapon(15, 10, 1.2f, 3.8f, 8);
                Destroy(gameObject);
            }
        }
    }
}
