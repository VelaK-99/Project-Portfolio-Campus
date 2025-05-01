using NUnit.Framework;
using System.Collections;
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
<<<<<<< Updated upstream
=======
    [Range(1, 5)][SerializeField] int interactDist;

    [Header("===== Weapons =====")]
    [SerializeField] public List<gunStats> arsenal = new List<gunStats>();

    public List<Hotkey_slots_UI> hotkey_Slots;
    Hotkey_Bar HotkeyBar;

    private List<GameObject> gunMODELS = new List<GameObject>();
    private GameObject current_gunMODEL;

    [SerializeField] GameObject gunModel;
    //[SerializeField] GameObject DUALmodel;

    [Range(1,10)] [SerializeField] int shootDamage;
    [Range(1, 10)] [SerializeField] int shootDist;
    [Range(0, 10)] [SerializeField] float shootRate;
    [Range(0, 200)] [SerializeField] int TotalAmmo;
    [Range(0, 10)] [SerializeField] float reloadTime;
    [SerializeField] int AmmoCapacity;
    [SerializeField] gunStats startingWeapon;

    [Header("===== Crouch/Slide =====")]
    [Range(1, 5)][SerializeField] float crouchHeight;
    [Range(1, 5)] [SerializeField] float crouchSpeedMod;
    [SerializeField] Transform cam;

    [Range(1, 6)] [SerializeField] float slideSpeed;
    [Range(0, 2)] [SerializeField] float slideDuration;

    [SerializeField] GameObject grenadePrefab;
    [SerializeField] Transform grenadeSpawnPoint;
    [SerializeField] float grenadeThrowForce;

    [Header("===== Audio =====")]
    [SerializeField] AudioClip[] audJump;
    [Range(0, 100)] [SerializeField] float audJumpVol;
    [SerializeField] AudioClip[] audHurt;
    [Range(0, 100)] [SerializeField] float audHurtVol;
    [SerializeField] AudioClip[] audStep;
    [Range(0, 100)] [SerializeField] float audStepVol;
    [SerializeField] AudioClip[] audReload;
    [Range(0, 100)] [SerializeField] float audReloadVol;
    [Range(0, 100)] [SerializeField] float shootSoundsVol;

    [Header("===== Aim Down Sights =====")]
    [SerializeField] float adsFov;
    [SerializeField] float normalFov; 

    [SerializeField] Vector3 adsCamPos;
    [SerializeField] float adsSpeed;

    [SerializeField] float recoilStrength; 
    [SerializeField] float recoilSpeed; 
    [SerializeField] Vector3 recoilDirection; 
    private Vector3 currentRecoil; 
    /*
    [SerializeField] int meleeDamage;
    [SerializeField] float meleeRate;
    [SerializeField] float meleeDist;
    */
>>>>>>> Stashed changes

    [SerializeField] int shootDamage = 1;
    [SerializeField] int interactDist;
    [SerializeField] int shootDist = 25;
    [SerializeField] float shootRate = 0.5f;
    [SerializeField] int TotalAmmo = 70;
    [SerializeField] float reloadTime = 1.2f;
    [SerializeField] int AmmoCapacity = 7;

    int bulletsInGun;
    int MaxAmmo = 100;

    int jumpCount;
    int HPOrig;

    float shootTimer;

    Vector3 moveDir;
    Vector3 playerVel;

    bool isSprinting;
    bool isReloading;

<<<<<<< Updated upstream
=======
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

    int baseSpeed;

    public Transform gun; 
    public Vector3 hipFirePos;
    public Vector3 adsGunPos;
    public Transform gunAimPos;
    public float gunAimSpeed = 10f;
    private Vector3 gunOriginalPos;


>>>>>>> Stashed changes
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
<<<<<<< Updated upstream
=======
        spawnPlayer();
        bulletsInGun = AmmoCapacity;
        UpdatePlayerUI();

        if (startingWeapon != null)
        { 
            arsenal.Add(startingWeapon);
            gunListPos = 0;
            startingWeapon.currentAmmo = startingWeapon.ammoCapacity;
            ChangeGun(gunListPos);
        }
        else
        {
            gunListPos = -1;
        }

        originalHeight = controller.height;
        originalCenter = controller.center;

        if (cam != null)
        {
            camOriginalPos = cam.localPosition;
            camDefaultPos = camOriginalPos;
            camCrouchPos = new Vector3(camOriginalPos.x, camOriginalPos.y - 0.5f, camOriginalPos.z); // tweak the offset in Inspector if needed
        }

        baseSpeed = speed;

        normalFov = Camera.main.fieldOfView;

        gunOriginalPos = gun.localPosition;
>>>>>>> Stashed changes
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        Sprint();

        if (Input.GetKeyDown(KeyCode.R) && !isReloading && bulletsInGun < AmmoCapacity)
        {
            StartCoroutine(Reload());
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

        if (Input.GetButton("Fire1") && shootTimer >= shootRate && bulletsInGun > 0 && !isReloading && !gameManager.instance.isPaused)
        {
            Shoot();
        }


        if(Input.GetButton("Interact"))
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

        if (jumpCount == 2 && Input.GetButton("Jump"))
        {
            playerVel.y = jetForce * Time.deltaTime;

            if (playerVel.y > jetMax)
            {
                playerVel.y = jetMax;
            }
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
    }

    public void AddAmmo(int amount)
    {
        TotalAmmo += amount;

        if(TotalAmmo > MaxAmmo) { TotalAmmo = MaxAmmo; }
    }

    public void AddHealth(int amount)
    {
        HP += amount;
        if(HP > HPOrig) HP = HPOrig;
    }


    
    public void Interact()
    {

        RaycastHit hitInteract; // Create ray for interaction

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInteract, interactDist, ~ignoreLayer))
        {
            Debug.Log(hitInteract.collider.name); // created a debug to see what is trying to interact with

            IInteract interaction = hitInteract.collider.GetComponent<IInteract>();

            if (interaction != null) interaction.Interact();
        }

    }

    public void TakeDamage(int amount)
    {
        HP -= amount;

        if (HP <= 0)
        {
            gameManager.instance.youLose();
        }
    }

    public int getOrigHP()
    {
        return HPOrig;
    }

    public int getCurHP()
    {
        return HP;
    }

    public void UpdateWeapon(int damage, int range, float fireRate, float ReloadTime, int ammoCapacity)
    {
        shootDamage = damage;
        shootDist = range;
        shootRate = fireRate;
        reloadTime = ReloadTime;
        AmmoCapacity = ammoCapacity;
    }

    public int GetMaxAmmo()
    {
        return MaxAmmo;
<<<<<<< Updated upstream
=======
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
        if (!arsenal.Contains(gun))
            {
                arsenal.Add(gun);
            }
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
        if (arsenal[gunListPos].totalAmmo > MaxAmmo)
        {
            arsenal[gunListPos].totalAmmo = MaxAmmo;
        }

        UpdatePlayerUI();
    } //When picking up Ammo

    public void RefillAllAmmo() // for the refill stash to refill all weapons
    {
        for (int gunListCount = 0; gunListCount < arsenal.Count; gunListCount++)
        {
            if (arsenal[gunListCount].totalAmmo < MaxAmmo) 
            { 
                arsenal[gunListCount].totalAmmo = arsenal[gunListCount].maxAmmo; 
                TotalAmmo = arsenal[gunListCount].totalAmmo;
            }
        }
        UpdatePlayerUI();
    }

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

        
        if (Input.GetButtonDown("num1")) HotkeyBar.EQUIPslot(0);
        if (Input.GetButtonDown("num2")) HotkeyBar.EQUIPslot(1);
        if (Input.GetButtonDown("num3")) HotkeyBar.EQUIPslot(2);
        
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

  
            for (int i = 0; i < hotkey_Slots.Count; i++)
            {
                if (i == gunListPos)
                {
                    hotkey_Slots[i].SetSLOT(arsenal[i]);
                }
                else
                {
                    hotkey_Slots[i].SetSLOT(null);
                }
            }
            
        }
    }

    void Crouch()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            isCrouching = !isCrouching;

            if (isCrouching)
            {
                controller.height = crouchHeight;
                controller.center = new Vector3(0, crouchHeight / 4, 0);
                speed = (int)(baseSpeed / crouchSpeedMod);

                if (cam != null)
                    cam.localPosition = camCrouchPos;
            }
            else
            {
                controller.height = originalHeight;
                controller.center = originalCenter;
                speed = baseSpeed;

                if (cam != null)
                    cam.localPosition = camOriginalPos;
            }
        }
    }

    void Slide()
    {
        if (Input.GetButtonDown("Slide"))
        {
            isSliding = true;
            isCrouching = true;

            slideTimer = slideDuration;

            controller.height = crouchHeight;
            controller.center = new Vector3(0, crouchHeight / 4, 0);
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
                speed = isSprinting ? baseSpeed * sprintMod : baseSpeed;


                if (cam != null)
                    cam.localPosition = camOriginalPos;
            }
        }
    }

    void ThrowGrenade()
    {
        if (Input.GetButtonDown("ThrowGrenade"))
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
        if (Input.GetButton("AimDownSights")) // hold right click to aim
        {
            isAiming = true;
        }
        else
        {
            isAiming = false;
        }

        // Keep camera fixed
        if (cam != null)
        {
            cam.localPosition = camDefaultPos;
        }

        // Move the gun between hip fire and ADS position
        Vector3 targetGunPos = isAiming ? adsGunPos : hipFirePos;
        gunAimPos.localPosition = Vector3.Lerp(gunAimPos.localPosition, targetGunPos, Time.deltaTime * gunAimSpeed);
    }

    /*    void Melee()
        {
            if (isMELEE)
            {
                meleeTimer += Time.deltaTime;

    //        if (Input.GetButtonDown("Melee"))
    //        {
    //            meleeTimer = 0;

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

    void ApplyRecoil()
    {
        
        currentRecoil = recoilDirection * recoilStrength;

       
        currentRecoil += new Vector3(Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f), 0);

        gun.localPosition += currentRecoil; 
>>>>>>> Stashed changes
    }
}


