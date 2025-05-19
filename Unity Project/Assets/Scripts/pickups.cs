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


    private void OnTriggerEnter(Collider other)
    {
        IPickup pickupable = other.GetComponent<IPickup>();

        if (pickupable != null && type == pickupType.healthPack)
        {
            //if (gameManager.instance.playerScript.getCurHP() < gameManager.instance.playerScript.getOrigHP())
            //{
                pickupable.HealthPickup(healthAmount);
                Destroy(gameObject);
                gameManager.instance.playerScript.UpdatePlayerUI();
            //}
        }

        if (pickupable != null && type == pickupType.ammoPack)
        {
            pickupable.AmmoPickup(ammoAmount);
            Destroy(gameObject);
        }

        if (pickupable != null && type == pickupType.gun)
        {
            gun.currentAmmo = gun.ammoCapacity;
            gun.totalAmmo = gun.maxAmmo;
            pickupable.GetGunStats(gun);
            Destroy(gameObject);
        }
    }
}