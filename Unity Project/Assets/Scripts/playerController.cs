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

    int bulletsInGun;
    int MaxAmmo = 100;


    int jumpCount;
    int HPOrig;
    int gunListPos;

    float shootTimer;

    Vector3 moveDir;
    Vector3 playerVel;

    bool isShotgun;
    bool isSprinting;
    bool isReloading;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
        bulletsInGun = AmmoCapacity;
        UpdatePlayerUI();

        arsenal.Add(startingWeapon);
        gunListPos = 0;
        startingWeapon.currentAmmo = startingWeapon.ammoCapacity;
        ChangeGun();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        Sprint();

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
            if (gameManager.instance.emptyGuntext != null)
                gameManager.instance.emptyGuntext.SetActive(true);
        }

        //Disabling Texts
        else
        {
            gameManager.instance.reloadGunText.SetActive(false);
            gameManager.instance.emptyGuntext.SetActive(false);
        }

        if (Input.GetButton("Interact"))
        {
            Interact();
        }

        SelectGun();
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


        for (int i = 0; i < pellets; i++)
        {
            Vector3 shootDirection = GetSpreadDirection(Camera.main.transform.forward, spreadAngle);

            if (Physics.Raycast(Camera.main.transform.position, shootDirection, out RaycastHit hit, shootDist, ~ignoreLayer))
            {
                Debug.DrawRay(Camera.main.transform.position, shootDirection * shootDist, Color.red, 1f);
                //Debug.Log(hit.collider.name);
                Instantiate(arsenal[gunListPos].hitEffect, hit.point, Quaternion.identity);

                IDamage dmg = hit.collider.GetComponent<IDamage>();
                if (dmg != null)
                    dmg.TakeDamage(shootDamage / pellets);
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

            if (interaction != null) interaction.Interact();

        }
        else if (gameManager.instance.textActive != null) // If the raycast does not detect the object it resets and clears the text.
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
        ChangeGun();
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

    void SelectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < arsenal.Count - 1)
        {
            gunListPos++;
            ChangeGun();
        }

        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunListPos > 0)
        {
            gunListPos--;
            ChangeGun();
        }
    }

    void ChangeGun()
    {
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


