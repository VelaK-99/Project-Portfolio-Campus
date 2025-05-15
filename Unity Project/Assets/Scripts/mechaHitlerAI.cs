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
            Instantiate(bullet, pos.position, pos.rotation);
        }
    }

    void FireBigCannon()
    {
        animator.SetTrigger("FireBigCannon");

        foreach(Transform pos in missilePos)
        {
            Instantiate(missile, pos.position, pos.rotation);
        }
    }

    void FireMortar()
    {
        Instantiate(rocket, rocketPos.position, rocketPos.rotation);
    }

    IEnumerator DpsPhase()
    {
        IsInDpsPhase = true;
        yield return new WaitForSeconds(10f);
        IsInDpsPhase = false;
    }

    void faceTarget()
    {
        Vector3 directionToLook = PlayerPos.position - transform.position;
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
