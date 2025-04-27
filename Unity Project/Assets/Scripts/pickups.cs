using Unity.Hierarchy;
using UnityEngine;

public class pickups : MonoBehaviour
{
    enum pickupType { healthPack, ammoPack, Pistol, AssaultRifle, Shotgun, Sniper }
    [SerializeField] int healthAmount;
    [SerializeField] int ammoAmount;
    [SerializeField] pickupType type;
    [SerializeField] float destroyTime;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
         if (type == pickupType.healthPack || type == pickupType.ammoPack)
                {
                    Destroy(gameObject, destroyTime);
                }
    }

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
                    gameManager.instance.playerScript.UpdatePlayerUI();
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
                player.UpdateWeapon(15, 15, 1.2f, 3.8f, 8);
                player.isShotgun = true;
                Destroy(gameObject);
            }

            if (player != null && type == pickupType.Sniper)
            {
                player.UpdateWeapon(30, 70, 2, 3f, 5);
                Destroy(gameObject);
            }
        }
    }
}