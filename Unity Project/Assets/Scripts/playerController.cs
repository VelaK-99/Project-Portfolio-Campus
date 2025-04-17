using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour, IDamage, IInteract
{
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] CharacterController controller;

    [SerializeField] int HP;
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] int jetForce;
    [SerializeField] int jetMax;
    [SerializeField] int gravity;
    [SerializeField] int interactDist;

    [SerializeField] int shootDamage = 1;
    [SerializeField] int shootDist = 25;
    [SerializeField] float shootRate = 0.5f;
    [SerializeField] int TotalAmmo = 70;
    [SerializeField] float reloadTime = 1.2f;
    [SerializeField] int AmmoCapacity = 7;
    [SerializeField] public bool isShotgun;

    [SerializeField] float crouchHeight;
    [SerializeField] float crouchSpeedMod;
    [SerializeField] Transform cam;

    [SerializeField] float slideSpeed;
    [SerializeField] float slideDuration;

    [SerializeField] GameObject grenadePrefab;
    [SerializeField] Transform grenadeSpawnPoint;
    [SerializeField] float grenadeThrowForce;

    int bulletsInGun;
    int MaxAmmo = 100;

    int jumpCount;
    int HPOrig;

    float shootTimer;

    Vector3 moveDir;
    Vector3 playerVel;

    bool isSprinting;
    bool isReloading;

    bool isCrouching;

    float originalHeight;
    Vector3 originalCenter;
    Vector3 camOriginalPos;
    Vector3 camCrouchPos;

    bool isSliding;
    float slideTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
        bulletsInGun = AmmoCapacity;
        UpdatePlayerUI();

        originalHeight = controller.height;
        originalCenter = controller.center;

        if (cam != null)
        {
            camOriginalPos = cam.localPosition;
            camCrouchPos = new Vector3(camOriginalPos.x, camOriginalPos.y - 0.5f, camOriginalPos.z); // tweak the offset in Inspector if needed
        }
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        Sprint();

        Crouch();

        Slide();

        ThrowGrenade();

        if (Input.GetKeyDown(KeyCode.R) && !isReloading && bulletsInGun < AmmoCapacity)
        {
            StartCoroutine(Reload());
        }

        if (bulletsInGun <= 0 && !isReloading && TotalAmmo > 0)
        {
            if (gameManager.instance.reloadGunText != null)
                gameManager.instance.reloadGunText.SetActive(true);
        }
        else
        {
            if (gameManager.instance.reloadGunText != null)
                gameManager.instance.reloadGunText.SetActive(false);
        }

        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * interactDist, Color.green);
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

        if (Input.GetButton("Fire1") && shootTimer >= shootRate && bulletsInGun > 0 && !isReloading && !gameManager.instance.isPaused && !isShotgun)
        {
            Shoot();
        }

        if (Input.GetButton("Fire1") && shootTimer >= shootRate && bulletsInGun > 0 && !isReloading && !gameManager.instance.isPaused && isShotgun)
        {
            ShootShotgun();
        }


        if (Input.GetButton("Interact"))
        {
            Interact();
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

    void ShootShotgun()
    {
        shootTimer = 0;
        int pellets = 10;
        float spreadAngle = 15f;


        for (int i = 0; i < pellets; i++)
        {
            Vector3 shootDirection = GetSpreadDirection(Camera.main.transform.forward, spreadAngle);

            if (Physics.Raycast(Camera.main.transform.position, shootDirection, out RaycastHit hit, shootDist, ~ignoreLayer))
            {
                Debug.DrawRay(Camera.main.transform.position, shootDirection * shootDist, Color.red, 1f);
                Debug.Log(hit.collider.name);

                IDamage dmg = hit.collider.GetComponent<IDamage>();
                if (dmg != null)
                    dmg.TakeDamage(shootDamage / pellets);
            }
        }
    }


    Vector3 GetSpreadDirection(Vector3 forward, float angle)
    {
        float spreadRadius = Mathf.Tan(angle * Mathf.Deg2Rad);
        Vector2 spread = Random.insideUnitCircle * spreadRadius;

        Vector3 direction = forward + Camera.main.transform.right * spread.x + Camera.main.transform.up * spread.y;
        return direction.normalized;
    } //Helper Function for shotgun spread

    IEnumerator Reload()
    {
        isReloading = true;
        gameManager.instance.reloadingGun.SetActive(true);

        yield return new WaitForSeconds(reloadTime);

        if(TotalAmmo >= AmmoCapacity)
        {
            TotalAmmo -= AmmoCapacity;
            bulletsInGun = AmmoCapacity;
        }
        else
        {
            bulletsInGun = TotalAmmo;
            TotalAmmo = 0;
        }
        isReloading = false;
        gameManager.instance.reloadingGun.SetActive(false);
    }

    public void AddAmmo(int amount)
    {
        TotalAmmo += amount;

        if(TotalAmmo > MaxAmmo) { TotalAmmo = MaxAmmo; }
    } // Adds ammo; Called in pickups

    public void AddHealth(int amount)
    {
        HP += amount;
        if(HP > HPOrig) HP = HPOrig;
    } //Adds health; Called in pickups

    public void Interact()
    {

        RaycastHit hitInteract; // Create ray for interaction

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInteract, interactDist, ~ignoreLayer))
        {
            Debug.Log(hitInteract.collider.name); // created a debug to see what is trying to interact with

            IInteract interaction = hitInteract.collider.GetComponent<IInteract>();

            if (interaction != null) interaction.Interact();

        }
        else if(gameManager.instance.textActive != null) // If the raycast does not detect the object it resets and clears the text.
        {
                gameManager.instance.textActive.SetActive(false);
                gameManager.instance.textActive = null; 
        }

    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        UpdatePlayerUI();
        StartCoroutine(FlashDamageScreen());

        if (HP <= 0)
        {
            gameManager.instance.youLose();
        }
    }

    public int getOrigHP()
    {
        return HPOrig;
    } // Getter for Player's max health

    public int getCurHP()
    {
        return HP;
    } // Getter for Player's cuurent health

    //Called in pickups. updates the shooting stats to the picked up weapon's
    public void UpdateWeapon(int damage, int range, float fireRate, float ReloadTime, int ammoCapacity)
    {
        shootDamage = damage;
        shootDist = range;
        shootRate = fireRate;
        reloadTime = ReloadTime;
        AmmoCapacity = ammoCapacity;
        bulletsInGun = ammoCapacity;
    }

    
    /*
    /// <summary>
    /// Test code for Hotkey bar to pick up weapons using an updated void UpdateWeapon method.
    /// Currently causes issues in pickups.cs
    /// </summary>
    /// <param name="thatWEAPON"></param>
    public void UpdateWeapon(Weapons thatWEAPON)
    {

        this.shootDamage = thatWEAPON.damage;
        this.shootDist = thatWEAPON.range;
        this.shootRate = thatWEAPON.fireRate;
        this.reloadTime = thatWEAPON.reloadTime;
        this.MaxAmmo = thatWEAPON.maxAmmo;
    

        //A test for the Hotkey system
        Weapons TESTweapon = new Weapons("TESTweapon", shootDamage, shootDist,
            shootRate, reloadTime, MaxAmmo, MaxAmmo);

        Hotkey_Bar hotkeyBAR = FindObjectOfType<Hotkey_Bar>();
        if (hotkeyBAR != null)
        {
            hotkeyBAR.AssignAvailableSLOT(TESTweapon);
        }
    }  
    */

    public int GetMaxAmmo()
    {
        return MaxAmmo;
    }  //Getter for Player's max ammo

    public void UpdatePlayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }

    IEnumerator FlashDamageScreen()
    {
        gameManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.playerDamageScreen.SetActive(false);
    }

    void Crouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;

            if (isCrouching)
            {
                controller.height = crouchHeight;
                controller.center = new Vector3(0, crouchHeight / 2, 0);
                speed /= (int)crouchSpeedMod;

                if (cam != null)
                    cam.localPosition = camCrouchPos;
            }
            else
            {
                controller.height = originalHeight;
                controller.center = originalCenter;
                speed *= (int)crouchSpeedMod;

                if (cam != null)
                    cam.localPosition = camOriginalPos;
            }
        }
    }

    void Slide()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && isSprinting && controller.isGrounded && !isSliding)
        {
            isSliding = true;
            isCrouching = true;

            slideTimer = slideDuration;

            controller.height = crouchHeight;
            controller.center = new Vector3(0, crouchHeight / 2, 0);
            speed = (int)slideSpeed;

            if (cam != null)
                cam.localPosition = camCrouchPos;
        }

        if (isSliding)
        {
            slideTimer -= Time.deltaTime;

            controller.Move(moveDir.normalized * speed * Time.deltaTime);

            if (slideTimer <= 0)
            {
                isSliding = false;
                isCrouching = false;

                controller.height = originalHeight;
                controller.center = originalCenter;
                speed = isSprinting ? speed / (int)crouchSpeedMod : speed / (int)(crouchSpeedMod * sprintMod); // Reset to normal or sprint speed

                if (cam != null)
                    cam.localPosition = camOriginalPos;
            }
        }
    }

    void ThrowGrenade()
    {
        if (Input.GetKeyDown(KeyCode.G) && grenadePrefab != null && grenadeSpawnPoint != null)
        {
            GameObject grenade = Instantiate(grenadePrefab, grenadeSpawnPoint.position, grenadeSpawnPoint.rotation);

            Rigidbody rb = grenade.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(grenadeSpawnPoint.forward * grenadeThrowForce, ForceMode.VelocityChange);
            }
        }
    }


}


