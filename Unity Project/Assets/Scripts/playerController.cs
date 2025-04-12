using System.Collections;
using UnityEngine;

public class playerScript : MonoBehaviour
{
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] CharacterController controller;

    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;

    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;
    [SerializeField] int totalAmmoCount;
    [SerializeField] float reloadTime;


    int jumpCount;

    int pistolCapacity = 7;
    int bulletsInGun;

    float shootTimer;

    Vector3 moveDir;
    Vector3 playerVel;

    bool isSprinting;
    bool isReloading;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bulletsInGun = pistolCapacity;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        Sprint();

        if (Input.GetKeyDown(KeyCode.R) && !isReloading && bulletsInGun < pistolCapacity && totalAmmoCount > 0)
        {
            StartCoroutine(Reload());
        }

        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);
    }

    void Movement()
    {

        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        moveDir = (Input.GetAxis("Horizontal") * transform.right) +
                 (Input.GetAxis("Vertical") * transform.forward);


        controller.Move(moveDir * speed * Time.deltaTime);

        Jump();

        playerVel.y -= gravity * Time.deltaTime;
        controller.Move(playerVel * Time.deltaTime);

        shootTimer += Time.deltaTime;

        if (Input.GetButton("Fire1") && shootTimer >= shootRate && bulletsInGun > 0 && !isReloading)
        {
            Shoot();
        }
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
        }
    }

    void Sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
        }
    }

    void Shoot()
    {
        shootTimer = 0;

        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            Debug.Log(hit.collider.name);

            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null) dmg.TakeDamage(shootDamage);
        }
        bulletsInGun--;
    }

    IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);

        if (totalAmmoCount > pistolCapacity)
        {
            totalAmmoCount -= pistolCapacity;
            bulletsInGun = pistolCapacity;
        }
        else
        {
            bulletsInGun = totalAmmoCount;
            totalAmmoCount = 0;
        }

        isReloading = false;
    }

    public void AddAmmo(int amount)
    {
        totalAmmoCount += amount;
    }
}

