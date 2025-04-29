using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour, IDamage, IInteract, IPickup
{
    [Header("===== Controls =====")]
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] CharacterController controller;

    [Header("===== Stats =====")]
    [SerializeField] int HP;
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] int jetForce;
    [SerializeField] int jetMax;
    [SerializeField] int gravity;
    [SerializeField] int interactDist;

    [Header("===== Weapons =====")]
    [SerializeField] List<gunStats> arsenal = new List<gunStats>();
    [SerializeField] GameObject gunModel;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;
    [SerializeField] int TotalAmmo;
    [SerializeField] float reloadTime;
    [SerializeField] int AmmoCapacity;
    [SerializeField] gunStats startingWeapon;

    [SerializeField] float crouchHeight;
    [SerializeField] float crouchSpeedMod;
    [SerializeField] Transform cam;

    [SerializeField] float slideSpeed;
    [SerializeField] float slideDuration;

    [SerializeField] GameObject grenadePrefab;
    [SerializeField] Transform grenadeSpawnPoint;
    [SerializeField] float grenadeThrowForce;

    [SerializeField] Vector3 adsCamPos;
    [SerializeField] float adsSpeed;

    /*
    [SerializeField] int meleeDamage;
    [SerializeField] float meleeRate;
    [SerializeField] float meleeDist;
    */

    int MaxAmmo;

    int jumpCount;
    int HPOrig;
    int gunListPos;
    public GameObject playerSpawnPos;

    float shootTimer;

    Vector3 moveDir;
    Vector3 playerVel;

    bool isShotgun;
    bool isSprinting;
    bool isReloading;

    bool isCrouching;

    float originalHeight;
    Vector3 originalCenter;
    Vector3 camOriginalPos;
    Vector3 camCrouchPos;

    bool isSliding;
    float slideTimer;

    Vector3 camDefaultPos;
    bool isAiming;

    int bulletsInGun;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
        spawnPlayer();
        bulletsInGun = AmmoCapacity;        
        UpdatePlayerUI();

        arsenal.Add(startingWeapon);
        gunListPos = 0;
        startingWeapon.currentAmmo = startingWeapon.ammoCapacity;
        ChangeGun(gunListPos);

        originalHeight = controller.height;
        originalCenter = controller.center;

        if (cam != null)
        {
            camOriginalPos = cam.localPosition;
            camDefaultPos = camOriginalPos;
            camCrouchPos = new Vector3(camOriginalPos.x, camOriginalPos.y - 0.5f, camOriginalPos.z); // tweak the offset in Inspector if needed
        }
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

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

        //Shooting
        if (Input.GetButton("Fire1") && shootTimer >= shootRate && bulletsInGun > 0 && !isReloading && !gameManager.instance.isPaused && !isShotgun)
        {
            Shoot();
        }

        //Shooting Shotgun
        else if (Input.GetButton("Fire1") && shootTimer >= shootRate && bulletsInGun > 0 && !isReloading && !gameManager.instance.isPaused && isShotgun)
        {
            ShootShotgun();
        }

        //Reload
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && bulletsInGun < AmmoCapacity && TotalAmmo != 0)
        {
            StartCoroutine(Reload());
        }

        //Displaying Reload Text
        if (bulletsInGun <= 0 && !isReloading && TotalAmmo > 0)
        {
            if (gameManager.instance.reloadGunText != null)
                gameManager.instance.reloadGunText.SetActive(true);
        }

        //Displaying Empty Gun Text
        else if (arsenal.Count > 0 && bulletsInGun <= 0 && !isReloading && TotalAmmo == 0)
        {
            if (gameManager.instance.emptyGunText != null)
                gameManager.instance.emptyGunText.SetActive(true);
        }

        //Disabling Texts
        else
        {
            gameManager.instance.reloadGunText.SetActive(false);
            gameManager.instance.emptyGunText.SetActive(false);
        }

        Interact();
        

        SelectGun();
        Sprint();
        Crouch();
        Slide();
        ThrowGrenade();
        AimDownSights();
        //Melee();
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
            //Debug.Log(hit.collider.name);

            Instantiate(arsenal[gunListPos].hitEffect, hit.point, Quaternion.identity);

            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null) dmg.TakeDamage(shootDamage);
        }
        arsenal[gunListPos].currentAmmo--;
        bulletsInGun = arsenal[gunListPos].currentAmmo;
        UpdatePlayerUI();
    }

    void ShootShotgun()
    {
        shootTimer = 0;
        int pellets = 10;
        float spreadAngle = 10f;

        if (isShotgun)
        {
            for (int i = 0; i < pellets; i++)
            {

                Vector3 shootDirection = GetSpreadDirection(Camera.main.transform.forward, spreadAngle);

                if (Physics.Raycast(Camera.main.transform.position, shootDirection, out RaycastHit hit, shootDist, ~ignoreLayer))
                {
                    Instantiate(arsenal[gunListPos].hitEffect, hit.point, Quaternion.identity);

                    Debug.DrawRay(Camera.main.transform.position, shootDirection * shootDist, Color.red, 1f);
                    Debug.Log(hit.collider.name);

                    IDamage dmg = hit.collider.GetComponent<IDamage>();
                    if (dmg != null)
                        dmg.TakeDamage(shootDamage / pellets);
                }
            }
        }
        arsenal[gunListPos].currentAmmo--;
        bulletsInGun = arsenal[gunListPos].currentAmmo;
        UpdatePlayerUI();
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
        gameManager.instance.reloadingGunText.SetActive(true);

        yield return new WaitForSeconds(reloadTime);

        if (TotalAmmo >= AmmoCapacity)
        {
            int reloadAmt = AmmoCapacity - bulletsInGun;
            TotalAmmo -= reloadAmt;
            bulletsInGun += reloadAmt;
        }
        else
        {
            bulletsInGun = TotalAmmo;
            TotalAmmo = 0;
        }
        isReloading = false;
        arsenal[gunListPos].currentAmmo = bulletsInGun;
        arsenal[gunListPos].totalAmmo = TotalAmmo;
        gameManager.instance.reloadingGunText.SetActive(false);

        UpdatePlayerUI();
    }

    public void Interact()
    {

        RaycastHit hitInteract; // Create ray for interaction

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInteract, interactDist, ~ignoreLayer))
        {
            Debug.Log(hitInteract.collider.name); // created a debug to see what is trying to interact with

            IInteract interaction = hitInteract.collider.GetComponent<IInteract>();

            if (interaction != null)
            { 
                interaction.Interact(); 
            }

        }
        else if(gameManager.instance.interactUI.activeSelf == true) // If the raycast does not detect the object it turns off the interaction text
        {
                gameManager.instance.interactUI.SetActive(false);
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
    } // Getter for Player's current health


    public int GetMaxAmmo()
    {
        return MaxAmmo;
    }  //Getter for Player's max ammo

    public void UpdatePlayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
        if (arsenal.Count > 0)
        {
            gameManager.instance.CurrentAmmo.text = arsenal[gunListPos].currentAmmo.ToString("F0");
            gameManager.instance.TotalAmmo.text = arsenal[gunListPos].totalAmmo.ToString("F0");
        }
    }

    IEnumerator FlashDamageScreen()
    {
        gameManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.playerDamageScreen.SetActive(false);
    }

    public void GetGunStats(gunStats gun)
    {
        arsenal.Add(gun);
        gunListPos = arsenal.Count - 1;
        ChangeGun(gunListPos);
    }

    public void HealthPickup(int amount)
    {
        HP += amount;
        if (HP > HPOrig) { HP = HPOrig; }

        UpdatePlayerUI();
    } //When picking up Health

    public void AmmoPickup(int amount)
    {
        arsenal[gunListPos].totalAmmo += amount;
        if (arsenal[gunListPos].totalAmmo > MaxAmmo) { arsenal[gunListPos].totalAmmo = MaxAmmo; }

        UpdatePlayerUI();
    } //When picking up Ammo

    public void SelectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < arsenal.Count - 1)
        {
            //gunListPos++;
            ChangeGun(gunListPos + 1);
        }

        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunListPos > 0)
        {
            //gunListPos--;
            ChangeGun(gunListPos - 1);
        }
    }

    public void ChangeGun(int index)
    {
        if (index >= 0 && index < arsenal.Count)
{
        gunListPos = index;

        shootDamage = arsenal[gunListPos].shootDmg;
        shootDist = arsenal[gunListPos].shootDist;
        shootRate = arsenal[gunListPos].shootRate;
        reloadTime = arsenal[gunListPos].reloadSpeed;
        AmmoCapacity = arsenal[gunListPos].ammoCapacity;
        bulletsInGun = arsenal[gunListPos].currentAmmo;
        TotalAmmo = arsenal[gunListPos].totalAmmo;
        MaxAmmo = arsenal[gunListPos].maxAmmo;
        isShotgun = arsenal[gunListPos].isShotgun;

        gunModel.GetComponent<MeshFilter>().sharedMesh = arsenal[gunListPos].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = arsenal[gunListPos].model.GetComponent<MeshRenderer>().sharedMaterial;

         UpdatePlayerUI();
        }
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

    void AimDownSights()
    {
        if (Input.GetMouseButton(1)) //hold right click to aim
        {
            isAiming = true;
        }
        else
        {
            isAiming = false;
        }

        if (cam != null)
        {
            Vector3 targetPos = isAiming ? adsCamPos : (isCrouching ? camCrouchPos : camDefaultPos);
            cam.localPosition = Vector3.Lerp(cam.localPosition, targetPos, Time.deltaTime * adsSpeed);
        }
    }

/*    void Melee()
    {
        if (isMELEE)
        {
            meleeTimer += Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.F) && meleeTimer >= meleeRate)
            {
                meleeTimer = 0;

                RaycastHit hit;

                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, meleeDist, ~ignoreLayer))
                {
                    Debug.Log("Melee hit: " + hit.collider.name);

                    IDamage dmg = hit.collider.GetComponent<IDamage>();
                    if (dmg != null)
                        dmg.TakeDamage(meleeDamage);
                }
            }
        }
    }*/

    public void spawnPlayer()
    {
        controller.transform.position = gameManager.instance.playerSpawnPos.transform.position;

        HP = HPOrig;
        UpdatePlayerUI();
    }
}


