using UnityEngine;

public class PlayerScript : MonoBehaviour, IDamage , IPickup, IInteract
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

    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] int interactDist;
    [SerializeField] float shootRate;

    [SerializeField] float slideDuration;
    [SerializeField] float slideSpeed;
    [SerializeField] float slideCooldown;



    int jumpCount;
    int HPOrig;

    float shootTimer;

    Vector3 moveDir;
    Vector3 playerVel;

    bool isSprinting;

    bool isSliding;
    bool canSlide;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
        canSlide = true;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        Sprint();

        Slide();

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

        if (Input.GetButton("Fire1") && shootTimer >= shootRate)
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

        if(jumpCount == 2 && Input.GetButton("Jump"))
        {
            playerVel.y = jetForce * Time.deltaTime;

            if(playerVel.y > jetMax)
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

    public void pickupHealth(int health)
    {
        HP += health;
        if (HP > HPOrig)
        {
            HP = HPOrig;
        }
    }

    public void pickupAmmo(int ammo)
    {
        // need to add ammo count or amount
    }

    public int getOrigHP()
    {
        return HPOrig;
    }

    public int getCurHP()
    {
        return HP;
    }

    float slideTimer;

    void Slide()
    {
        if (Input.GetButtonDown("Crouch") && isSprinting && controller.isGrounded && canSlide)
        {
            isSliding = true;
            canSlide = false;
            slideTimer = 0;
        }

        if (isSliding)
        {
            slideTimer += Time.deltaTime;

            controller.Move(moveDir.normalized * slideSpeed * Time.deltaTime);

            if (slideTimer >= slideDuration)
            {
                isSliding = false;
                Invoke("ResetSlide", slideCooldown);
            }
        }
    }


}


