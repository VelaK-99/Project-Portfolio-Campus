using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class PlayerScript : MonoBehaviour, IDamage, IInteract, IPickup
{
    [Header("===== Controls =====")]
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] CharacterController controller;
    [SerializeField] AudioSource aud;
    [SerializeField] Animator animator;
    [SerializeField] public Animator handsAnimator;
    [SerializeField] int animTranSpeed;
    public GameObject headPosition;

    [Header("===== Stats =====")]
    [Range(1, 100)][SerializeField] int HP;
    [Range(1, 10)][SerializeField] int speed;
    [Range(1, 3)][SerializeField] int sprintMod;
    [Range(1, 20)][SerializeField] int jumpSpeed;
    [Range(1, 3)][SerializeField] int jumpMax;
    public int meleeDamage = 5;
    public float meleeRange = 2f;
    public float meleeCooldown = 3f;
    private float meleeTimer = 0;
    public LayerMask Enemylayer;
    [Range(1, 10)][SerializeField] int jetForce;
    [Range(1, 10)][SerializeField] int jetMax;
    [SerializeField] int gravity;
    [Range(1, 5)][SerializeField] int interactDist;

    [Header("===== Weapons =====")]
    [SerializeField] public List<gunStats> arsenal = new List<gunStats>(); 

    public List<Hotkey_slots_UI> hotkey_Slots;

    [SerializeField] GameObject gunModel;

    /*
    [SerializeField] GameObject DUALmodel;
    public gunStats SecondaryGUN;
    public bool isDUALwielding = false;
    */

    public Transform laserOrigin;
    public float laserDuration = 0.05f;
    LineRenderer laserLine;

    int shootDamage;
    int shootDist;
    float shootRate;
     int TotalAmmo;
    float reloadTime;
    int AmmoCapacity;
    [SerializeField] gunStats startingWeapon;

    [Header("===== Crouch/Slide =====")]
    float crouchHeight = 2f;
    float crouchSpeedMod = 2f;
    [SerializeField] public Transform cam;

    float slideSpeed = 6f;
    float slideDuration = 0.6f;

    float slideFov = 80f;
    float slideFovSpeed = 7f;

    [Header("===== Grenade =====")]
    [SerializeField] GameObject grenadePrefab;
    [SerializeField] Transform grenadeSpawnPoint;
    float grenadeThrowForce = 100f;

    [Header("===== Audio =====")]
    [SerializeField] AudioClip[] audJump;
    [Range(0, 100)][SerializeField] float audJumpVol;
    [SerializeField] AudioClip[] audHurt;
    [Range(0, 100)][SerializeField] float audHurtVol;
    [SerializeField] AudioClip[] audStep;
    [Range(0, 100)][SerializeField] float audStepVol;
    [SerializeField] AudioClip[] audReload;
    [Range(0, 100)][SerializeField] float audReloadVol;
    [Range(0, 100)][SerializeField] float shootSoundsVol;

    [Header("===== Aim Down Sights =====")]
    float adsFov = 40f;
    float normalFov = 80f;

    [SerializeField] Vector3 adsCamPos;

    float recoilStrength = 0.5f;
    float recoilSpeed = 6f;
    [SerializeField] Vector3 recoilDirection;
    private Vector3 currentRecoil;

    [SerializeField] FreezeAbility freezeAbility;
    


    float bobFrequency = 0f;
    float bobAmplitude = 0f;
    float bobLerpSpeed = 4f;
    float sprintBobFrequency = 1.5f;
    float sprintBobAmplitude = 0.2f;
    float walkBobFrequency = 0.04f;
    float walkBobAmplitude = 0.03f;


    float camwalkBobAmplitude = 0f;
    float camwalkBobFrequency = 0f;
    float camsprintBobAmplitude = 0f;
    float camsprintBobFrequency = 0f;


    float bobTimer;

    //private Vector3 currentRecoil;
    private float cambobTimer = 0f;
    private Vector3 bobcamOriginalPos;


    Vector3 gunBobOffset;


    int MaxAmmo = 100;

    int jumpCount;
    int HPOrig;
    int gunListPos;
    public GameObject playerSpawnPos;

    float shootTimer;

    Vector3 moveDir;
    Vector3 playerVel;

    bool canMOVE;

    bool isPlayingStep;
    bool isShotgun;
    bool isSprinting;
    bool isReloading;
    bool isMoving;

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

    public Transform gun; // assign GunPos in the inspector
    //public Transform DUALgun;
    public Vector3 hipFirePos;
    public Vector3 adsGunPos;
    public float gunAimSpeed = 10f;
    private Vector3 gunOriginalPos;

    [Header("===== Rage =====")]
    [SerializeField] RageUI rageUI;
    public float moveSpeed;
    public float gunDamage;
    public float fireRate;

    public float maxRage = 100f;
    public float currentRage = 0f;
    public float rageDuration = 5f; // how long rage lasts 
    public float rageDamageMultiplier = 2f;
    public float rageSpeedMultiplier = 1.5f;

    private bool isRaging = false;
    private float rageTimer = 0f;

    public float normalSpeed = 5f;
    private float currentSpeed;

    private float normalDamage = 10f;
    private float currentDamage;





    /*
    /// <summary>
    /// Assign in the inspector
    /// </summary>
    public GameObject Dropped_Weapon;
    /// <summary>
    /// Where the weapon drops (in front of the player)
    /// </summary>
    public Transform dropPOINT;
    */


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canMOVE = true;
        animator = GetComponent<Animator>();
        HPOrig = HP;
        spawnPlayer();
        bulletsInGun = AmmoCapacity;
        UpdatePlayerUI();
        laserLine = GetComponent<LineRenderer>();
        List<gunStats> loadedGuns = gameManager.instance.LoadGame();
        if (loadedGuns != null && loadedGuns.Count > 0)
        {
            arsenal = loadedGuns;
            gunListPos = 0;
            ChangeGun(gunListPos);
        }
        else if (startingWeapon != null)
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
            cam.localPosition = new Vector3(0, 2.21f, 0);
            camOriginalPos = cam.localPosition;
            camDefaultPos = camOriginalPos;
            camCrouchPos = new Vector3(camOriginalPos.x, camOriginalPos.y - 0.5f, camOriginalPos.z); // tweak the offset in Inspector if needed
        }

        baseSpeed = speed;

        normalFov = Camera.main.fieldOfView;
        gunOriginalPos = gun.localPosition;

        Camera.main.fieldOfView = normalFov;

        if (cam != null)
        {
            bobcamOriginalPos = cam.localPosition;
        }

        currentDamage = normalDamage;
        currentSpeed = normalSpeed;
        currentRage = 0f;

        // Ensure rage screen is off at start
        if (gameManager.instance.playerRageScreen != null)
            gameManager.instance.playerRageScreen.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {

        OnAnimLocomotion();

        Movement();

        meleeTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (freezeAbility != null)
            {
                handsAnimator.SetTrigger("LongCast");
                freezeAbility.ActivateFreeze();
            }
        }

        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * interactDist, Color.green);

        Vector3 targetGunPos = isAiming ? adsGunPos : hipFirePos;

        if (isAiming)
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, adsFov, Time.deltaTime * 8f);
        }
        else
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, normalFov, Time.deltaTime * 8f);
        }


        if (currentRecoil != Vector3.zero)
        {

            gun.localPosition = Vector3.Lerp(gun.localPosition, gunOriginalPos, Time.deltaTime * recoilSpeed);

            /*
            if (DUALgun != null)
            {
                Vector3 dualOFFset = new Vector3(-0.442f, -0.322f, 0.652f);
                Vector3 dualTARGETpos = targetGunPos + dualOFFset;
                DUALgun.localPosition = Vector3.Lerp(DUALgun.localPosition, dualTARGETpos, Time.deltaTime * recoilSpeed);
            }
            */

            currentRecoil = Vector3.Lerp(currentRecoil, Vector3.zero, Time.deltaTime * recoilSpeed);
        }

        float targetFov = normalFov;

        if (isSliding)
            targetFov = slideFov;
        else if (isAiming)
            targetFov = adsFov;

        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFov, Time.deltaTime * slideFovSpeed);

        GunBobbing();

        if (controller.isGrounded && moveDir.magnitude > 0.1f && !isSliding)
        {
            float currentFrequency = isSprinting ? sprintBobFrequency : walkBobFrequency;
            float currentAmplitude = isSprinting ? sprintBobAmplitude : walkBobAmplitude;

            bobTimer += Time.deltaTime * currentFrequency;

            float bobX = Mathf.Cos(bobTimer) * currentAmplitude;
            float bobY = Mathf.Abs(Mathf.Sin(bobTimer * 2)) * currentAmplitude;
            bobY = Mathf.Clamp(bobY, -0.1f, 0.1f);

            Vector3 baseGunPos = isAiming ? adsGunPos : hipFirePos;
            Vector3 bobOffset = new Vector3(bobX, bobY, 0);
             targetGunPos = baseGunPos + bobOffset;

            gun.localPosition = Vector3.Lerp(gun.localPosition, targetGunPos, Time.deltaTime * gunAimSpeed);

            //updateDUALguns(targetGunPos);
                
            /*
            if (DUALgun != null)
            {
                DUALgun.localPosition = Vector3.Lerp(DUALgun.localPosition, targetGunPos, Time.deltaTime * gunAimSpeed);
            }
            */

        }
        else
        {
            bobTimer = 0;
             targetGunPos = isAiming ? adsGunPos : hipFirePos;
            gun.localPosition = Vector3.Lerp(gun.localPosition, targetGunPos, Time.deltaTime * gunAimSpeed);

            //updateDUALguns(targetGunPos);

            /*
            if(DUALgun != null)
            {
                DUALgun.localPosition = Vector3.Lerp(DUALgun.localPosition, targetGunPos, Time.deltaTime * gunAimSpeed);
            }
            */
        }

        CameraBobbing();
    }    

    // Call this method whenever player does or takes damage to gain rage
 

    void Movement()
    {
        if (!canMOVE) return;

        if (controller.isGrounded)
        {
            if (moveDir.normalized.magnitude > 0.3f && !isPlayingStep)
            {
                StartCoroutine(playStep());
            }
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        moveDir = (Input.GetAxis("Horizontal") * transform.right) +
                 (Input.GetAxis("Vertical") * transform.forward);

        float regSpeed = Mathf.Clamp01(moveDir.magnitude) * speed;

        if(regSpeed > 0.5f && regSpeed < 0.6f) { regSpeed = 0.5f; }
        animator.SetFloat("speed", regSpeed, 0.05f, Time.deltaTime);

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
        Melee();
        Crouch();
        Slide();
        ThrowGrenade();
        AimDownSights();
    }

    IEnumerator playStep()
    {
        isPlayingStep = true;
        aud.PlayOneShot(audStep[Random.Range(0, audStep.Length)], audStepVol);

        if (isSprinting)
        {
            yield return new WaitForSeconds(0.3f);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);

        }
        isPlayingStep = false;
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
            jumpCount++;
            playerVel.y = jumpSpeed;
        }
    }

    void Sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            isSprinting = true;
            speed *= sprintMod;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isSprinting = false;
        }
    }

    void Shoot()
    {
        if (arsenal[gunListPos].isElectricOrb)
        {
            ShootElectricOrb();
            return;
        }

        if (arsenal[gunListPos].GunName == "Laser")
        {
            laserLine.SetPosition(0, laserOrigin.position);
            Vector3 rayOrigin = controller.transform.position;
            
        }
        shootTimer = 0;
        aud.PlayOneShot(arsenal[gunListPos].shootSounds[Random.Range(0, arsenal[gunListPos].shootSounds.Length)], arsenal[gunListPos].shootSoundVol);
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {

            Instantiate(arsenal[gunListPos].hitEffect, hit.point, Quaternion.identity);
            if (arsenal[gunListPos].GunName == "Laser")
            {
                laserLine.SetPosition(1, hit.point);
                StartCoroutine(ShootLaser());
            }

            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null) dmg.TakeDamage(shootDamage);
        }
        else if(arsenal[gunListPos].GunName == "Laser")
        {
            laserLine.SetPosition(1, controller.transform.position + (cam.transform.forward * arsenal[gunListPos].shootDist));
            StartCoroutine(ShootLaser());
        }
        arsenal[gunListPos].currentAmmo--;
        bulletsInGun = arsenal[gunListPos].currentAmmo;
        UpdatePlayerUI();
        //DUALshoot();
        ApplyRecoil();
    }

    void ShootElectricOrb()
    {
        shootTimer = 0;
        aud.PlayOneShot(arsenal[gunListPos].shootSounds[Random.Range(0, arsenal[gunListPos].shootSounds.Length)], arsenal[gunListPos].shootSoundVol);

        Transform shootPoint = Camera.main.transform; // Change this to your actual shoot point if it's a separate object

        // Raycast from the center of the screen (crosshair)
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint = ray.GetPoint(arsenal[gunListPos].shootDist); // Default direction

        if (Physics.Raycast(ray, out hit, arsenal[gunListPos].shootDist))
        {
            targetPoint = hit.point;
        }

        
        GameObject orb = Instantiate(arsenal[gunListPos].electricOrbPrefab, shootPoint.position, Quaternion.identity);

        
        Vector3 direction = (targetPoint - shootPoint.position).normalized;

        
        orb.transform.rotation = Quaternion.LookRotation(direction);

        
        Rigidbody rb = orb.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction * arsenal[gunListPos].electricOrbSpeed;
        }

        
        ElectricGun orbScript = orb.GetComponent<ElectricGun>();
        if (orbScript != null)
        {
            orbScript.SetDamage(arsenal[gunListPos].electricOrbDamage);
            orbScript.SetLifetime(arsenal[gunListPos].electricOrbLifetime);
        }
    }

    /*
    void DUALshoot()
    {
        if (SecondaryGUN == null || SecondaryGUN.currentAmmo <= 0)
        {
            return;
        }

        aud.PlayOneShot(SecondaryGUN.shootSounds[Random.Range(0, SecondaryGUN.shootSounds.Length)], shootSoundsVol);

        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            Instantiate(SecondaryGUN.hitEffect, hit.point, Quaternion.identity);

            IDamage DMG = hit.collider.GetComponent<IDamage>();
            if (DMG != null)
            {
                DMG.TakeDamage(shootDamage);
            }
        }

        arsenal[gunListPos].currentAmmo -= 1;
        bulletsInGun = arsenal[gunListPos].currentAmmo;
        ApplyRecoil();
        UpdatePlayerUI();
    }
    */

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
                    aud.PlayOneShot(arsenal[gunListPos].shootSounds[Random.Range(0, arsenal[gunListPos].shootSounds.Length)], arsenal[gunListPos].shootSoundVol);
                    Instantiate(arsenal[gunListPos].hitEffect, hit.point, Quaternion.identity);

                    Debug.DrawRay(Camera.main.transform.position, shootDirection * shootDist, Color.red, 1f);

                    IDamage dmg = hit.collider.GetComponent<IDamage>();
                    if (dmg != null)
                        dmg.TakeDamage(shootDamage / pellets);
                }
            }
        }
        arsenal[gunListPos].currentAmmo--;
        bulletsInGun = arsenal[gunListPos].currentAmmo;
        //DUALshoot();
        ApplyRecoil();
        UpdatePlayerUI();
    }

    IEnumerator ShootLaser()
    {
        laserLine.enabled = true;
        yield return new WaitForSeconds(laserDuration);
        laserLine.enabled = false;
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
        aud.PlayOneShot(audReload[Random.Range(0, audReload.Length)], audReloadVol);
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
            IInteract interaction = hitInteract.collider.GetComponent<IInteract>();

            if (interaction != null)
            {
                interaction.Interact();
            }

        }
        else if (gameManager.instance.interactUI.activeSelf == true) // If the raycast does not detect the object it turns off the interaction text
        {
            gameManager.instance.interactUI.SetActive(false);
        }

    }

    public void TakeDamage(int amount)
    {

        HP -= amount;
        aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);
        UpdatePlayerUI();
        StartCoroutine(FlashDamageScreen());

        if (HP <= 0)
        {
            gameManager.instance.youLose();
        }
    }


    public void Stun(float duration, Vector3 knockbackDIR)
    {
        StartCoroutine(StunCuroutine(duration, knockbackDIR));
    }

    IEnumerator StunCuroutine(float duration, Vector3 knockbackDIR)
    {
        canMOVE = false; //halt movement

        float timer = 0f;
        Vector3 velocity = knockbackDIR;

        while (timer < duration)
        {
            controller.Move(velocity * Time.deltaTime); //Move away
            timer += Time.deltaTime;
            yield return null;
        }
        canMOVE = true; //resume movement
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

            Hotkey_slots_UI.instance.UpdateAmmo(arsenal[gunListPos]);
            Hotkey_slots_UI.instance.SetSLOT(arsenal[gunListPos]);
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

        //dualWIELD(gun);
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

    public void Melee()
    {
        if (Input.GetKeyDown(KeyCode.Q) && meleeTimer <= 0)
        {
            animator.SetTrigger("Melee");
            meleeTimer = meleeCooldown;

            RaycastHit hit;
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, meleeRange, ~ignoreLayer))
                {
                    IDamage dmg = hit.collider.GetComponent<IDamage>();

                    if (dmg != null) dmg.TakeDamage(meleeDamage);
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
        if (cam != null && isAiming)
        {
            cam.localPosition = camDefaultPos;
        }

        // Move the gun between hip fire and ADS position
        Vector3 targetGunPos = isAiming ? adsGunPos : hipFirePos;

    }

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

        /*
        if (DUALgun != null)
        {
            DUALgun.localPosition += currentRecoil;
        }
        */
    }

    void GunBobbing()
    {
        float frequency = isSprinting ? sprintBobFrequency : bobFrequency;
        float amplitude = isSprinting ? sprintBobAmplitude : bobAmplitude;

        if (controller.isGrounded && moveDir.magnitude > 0.1f && !isAiming)
        {
            bobTimer += Time.deltaTime * frequency;

            float bobX = Mathf.Cos(bobTimer) * amplitude;
            float bobY = Mathf.Sin(bobTimer * 2) * amplitude;

            gunBobOffset = new Vector3(bobX, bobY, 0);
        }
        else
        {
            bobTimer = 0f;
            gunBobOffset = Vector3.Lerp(gunBobOffset, Vector3.zero, Time.deltaTime * bobLerpSpeed);
        }

        Vector3 targetGunPos = (isAiming ? adsGunPos : hipFirePos) + gunBobOffset;
        gun.localPosition = Vector3.Lerp(gun.localPosition, targetGunPos, Time.deltaTime * gunAimSpeed);

        //updateDUALguns(targetGunPos);
    }

    void CameraBobbing()
    {
        if (controller.isGrounded && moveDir.magnitude > 0.1f && !isSliding)
        {
            float currentFrequency = isSprinting ? camsprintBobFrequency : camwalkBobFrequency;
            float currentAmplitude = isSprinting ? camsprintBobAmplitude : camwalkBobAmplitude;

            cambobTimer += Time.deltaTime * currentFrequency;

            // X-axis and Y-axis bobbing
            float bobX = Mathf.Cos(bobTimer) * currentAmplitude;
            float bobY = Mathf.Abs(Mathf.Sin(bobTimer * 2)) * currentAmplitude;
            bobY = Mathf.Clamp(bobY, -0.1f, 0.1f); // Keeps Y bobbing small

            // Apply bobbing effect to camera
            Vector3 bobOffset = new Vector3(bobX, bobY, 0);
            cam.localPosition = bobcamOriginalPos + bobOffset;
        }
        else
        {
            // Reset bob timer and position if not moving
            cambobTimer = 0f;
            cam.localPosition = bobcamOriginalPos;
        }
    }

    void OnAnimLocomotion()
    {
        float speed = controller.velocity.magnitude;
        float animSpeedCur = animator.GetFloat("speed");

        if(speed < 0.5f) { speed = 0f; }
        animator.SetFloat("speed", speed);
    }

    //public void ActivateRage(float duration)
    //{
    //    StartCoroutine(RageRoutine(duration));
    //}

    //IEnumerator RageRoutine(float duration)
    //{
      
    //    float originalSpeed = moveSpeed;
    //    float originalDamage = gunDamage;
    //    float originalFireRate = fireRate;

    //    moveSpeed *= 2;
    //    gunDamage *= 2;
    //    fireRate /= 2;

    //    // Turn on rage UI
    //    gameManager.instance.playerRageScreen.SetActive(true);

    //    yield return new WaitForSeconds(duration);

    //    // Reset stats
    //    moveSpeed = originalSpeed;
    //    gunDamage = originalDamage;
    //    fireRate = originalFireRate;

    //    // Turn off rage UI
    //    gameManager.instance.playerRageScreen.SetActive(false);
    //}

   public void GainRage(float amount)
{
    if (isRaging) return;

    currentRage = Mathf.Min(currentRage + amount, maxRage);
    rageUI.UpdateRageBar(currentRage, maxRage);
}

    void StartRage()
    {
        isRaging = true;
        rageTimer = rageDuration;
        currentRage = 0f;

        // Boost damage and speed
        currentDamage = normalDamage * rageDamageMultiplier;
        currentSpeed = normalSpeed * rageSpeedMultiplier;

        // Show rage red border
        if (gameManager.instance.playerRageScreen != null)
            gameManager.instance.playerRageScreen.SetActive(true);

    
    }

    void EndRage()
    {
        isRaging = false;

        // Reset damage and speed
        currentDamage = normalDamage;
        currentSpeed = normalSpeed;

        // Hide rage UI
        if (gameManager.instance.playerRageScreen != null)
            gameManager.instance.playerRageScreen.SetActive(false);


    }

    }
