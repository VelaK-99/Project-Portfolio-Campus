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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
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
    }
}


