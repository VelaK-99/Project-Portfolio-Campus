using UnityEngine;
using System.Collections;

public class mechaHitlerAI : MonoBehaviour, IDamage
{
    [Header("===== Dependencies =====")]
    private Transform PlayerPos;
    [SerializeField] Animator animator;
    [SerializeField] Renderer model;

    [Header("===== Weapons =====")]
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject missile;
    [SerializeField] GameObject rocket;

    [SerializeField] Transform[] bulletPos;
    [SerializeField] Transform[] missilePos;
    [SerializeField] Transform rocketPos;

    [Header("===== Attack Stats =====")]
    [SerializeField] int rangeForBullets;
    [SerializeField] int rangeForMissiles;
    [SerializeField] int mortarInterval;

    [Header("===== MechaHitler Stats =====")]
    [SerializeField] int Health;
    private int originalHealth;
    [SerializeField] float fireRate;
    [SerializeField] float GoIntoDPSPhaseEvery;
    [SerializeField] float Phase2Multiplier = 5f;
    private float fireTimer;
    private float mortarTimer;
    private float dpsPhaseCooldown;
    private bool IsInDpsPhase = false;


    void Start()
    {
        originalHealth = Health;
        animator = GetComponent<Animator>();
        animator.SetTrigger("SpawnedIn");
    }

    // Update is called once per frame
    void Update()
    {
        PlayerPos = gameManager.instance.player.transform;
        faceTarget();
        float distance = Vector3.Distance(transform.position, PlayerPos.position);

        if(Health <= (originalHealth / 2))
        {
            model.material.color = Color.red;
            mortarTimer += Time.deltaTime * Phase2Multiplier;
            fireTimer += Time.deltaTime * Phase2Multiplier;
            dpsPhaseCooldown += Time.deltaTime / Phase2Multiplier;
        }
        else
        {
            mortarTimer += Time.deltaTime;
            fireTimer += Time.deltaTime;
            dpsPhaseCooldown += Time.deltaTime;
        }

        if(mortarTimer >= mortarInterval)
        {
            FireMortar();
            mortarTimer = 0f;
        }

        if(fireTimer >= fireRate && !IsInDpsPhase)
        {
            if(distance <= rangeForBullets)
            {
                FireSmallCannon();
            }

            else if(distance <= rangeForMissiles)
            {
                FireBigCannon();
            }

            fireTimer = 0f;
        }

        if(dpsPhaseCooldown >= GoIntoDPSPhaseEvery)
        {
            StartCoroutine(DpsPhase());
        }
    }

    void FireSmallCannon()
    {
        animator.SetTrigger("FireSmallCannon");

        foreach(Transform pos in bulletPos)
        {
            Vector3 targetDir = (PlayerPos.position + Vector3.up * 1.5f) - pos.position;
            Quaternion rot = Quaternion.LookRotation(targetDir);

            Instantiate(bullet, pos.position, rot);
        }
    }

    void FireBigCannon()
    {
        animator.SetTrigger("FireBigCannon");

        foreach(Transform pos in missilePos)
        {
            Vector3 targetDir = (PlayerPos.position + Vector3.up * 2.5f) - pos.position;
            Quaternion rot = Quaternion.LookRotation(targetDir);

            Instantiate(missile, pos.position, rot);
        }
    }

    void FireMortar()
    {
        Vector3 targetDir = (PlayerPos.position + Vector3.up * 2.5f) - rocketPos.position;
        Quaternion rot = Quaternion.LookRotation(targetDir);

        Instantiate(rocket, rocketPos.position, rot);
    }

    IEnumerator DpsPhase()
    {
        IsInDpsPhase = true;
        yield return new WaitForSeconds(10f);
        IsInDpsPhase = false;
    }

    void faceTarget()
    {
        Vector3 directionToLook = new Vector3 (PlayerPos.position.x, PlayerPos.position.y - 3f, PlayerPos.position.z) - transform.position;
        Quaternion rot = Quaternion.LookRotation(directionToLook);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
    }

    void ShootBigCanonA()
    {
        //Leave Empty
    }

    void ShootBigCanonB()
    {
        //Leave Empty
    }

    void ShootSmallCanonA()
    {
        //Leave Empty
    }

    void ShootSmallCanonB()
    {
        //Leave Empty
    }

    void IDamage.TakeDamage(int amount)
    {
        Health -= amount;

        if (Health < 0)
        {
            Destroy(gameObject);
        }
    }
}
