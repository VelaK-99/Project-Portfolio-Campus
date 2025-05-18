using UnityEngine;
using System.Collections;

public class mechaHitlerAI : MonoBehaviour, IDamage, IBoss
{
    [Header("===== Dependencies =====")]
    private Transform PlayerPos;
    [SerializeField] Animator animator;
    [SerializeField] Renderer model;
    [SerializeField] AudioSource bulletAudio;
    [SerializeField] AudioSource missileAudio;
    [SerializeField] AudioSource mortarAudio;
    [SerializeField] AudioSource DeathAudio;

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
    private bool IsDead = false;
    public bool IsActive = false;

    private Color origColor;

    [Header("===== Freeze Stuff =====")]
    private bool isFrozen = false;
    private float freezeTimer = 0f;
    FreezeAbility freezeAbility;

    public int CurrentHealth => Health;

    public int MaxHealth => originalHealth;

    void Start()
    {
        gameManager.instance.ShowBossHealthBar(this);

        originalHealth = Health;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isFrozen)
        {
            freezeTimer -= Time.deltaTime;
            if (freezeTimer <= 0f)
            {
                Unfreeze();
            }
            return;
        }

        PlayerPos = gameManager.instance.player.transform;
        faceTarget();
        float distance = Vector3.Distance(transform.position, PlayerPos.position);

        if (Health <= (originalHealth / 2))
        {
            origColor = model.material.color;
            model.material.color = Color.Lerp(origColor, Color.red, Time.deltaTime * 1);
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

        if (mortarTimer >= mortarInterval && !IsInDpsPhase && !IsDead && IsActive)
        {
            FireMortar();
            mortarTimer = 0f;
        }

        if (fireTimer >= fireRate && !IsInDpsPhase && !IsDead && IsActive)
        {
            if (distance <= rangeForBullets)
            {
                FireSmallCannon();
            }

            else if (distance <= rangeForMissiles)
            {
                FireBigCannon();
            }

            fireTimer = 0f;
        }

        if (dpsPhaseCooldown >= GoIntoDPSPhaseEvery)
        {
            StartCoroutine(DpsPhase());
        }
    }

    public void ApplyFreeze(float duration)
    {
        if (isFrozen) return;

        isFrozen = true;
        freezeTimer = duration;

        animator.enabled = false;
        
    }

    public void Unfreeze()
    {
        isFrozen = false;
        animator.enabled = true;
        this.enabled = true;
    }

    IEnumerator freezeBlue()
    {
        model.material.color = Color.cyan;
        yield return new WaitForSeconds(freezeAbility.bossFreezeDuration);
        model.material.color = origColor;
    }

    void FireSmallCannon()
    {
        animator.SetTrigger("FireSmallCannon");

        if (bulletAudio != null) { bulletAudio.Play(); }

        foreach (Transform pos in bulletPos)
        {
            Vector3 targetDir = (PlayerPos.position + Vector3.up * 1.5f) - pos.position;
            Quaternion rot = Quaternion.LookRotation(targetDir);

            Instantiate(bullet, pos.position, rot);
        }
    }

    void FireBigCannon()
    {
        animator.SetTrigger("FireBigCannon");

        if (missileAudio != null) { missileAudio.Play(); }

        foreach (Transform pos in missilePos)
        {
            Vector3 targetDir = (PlayerPos.position + Vector3.up * 2.5f) - pos.position;
            Quaternion rot = Quaternion.LookRotation(targetDir);

            Instantiate(missile, pos.position, rot);
        }
    }

    void FireMortar()
    {
        if (mortarAudio != null) { mortarAudio.Play(); }

        Vector3 targetDir = (PlayerPos.position + Vector3.up * 1f) - rocketPos.position;
        Quaternion rot = Quaternion.LookRotation(targetDir);

        Instantiate(rocket, rocketPos.position, rot);
    }

    IEnumerator DpsPhase()
    {
        IsInDpsPhase = true;
        yield return new WaitForSeconds(10f);
        IsInDpsPhase = false;
    }

    IEnumerator MechDeath()
    {
        IsDead = true;
        if (DeathAudio != null) { DeathAudio.Play(); }
        animator.SetTrigger("Death");
        yield return new WaitForSeconds(1.5f);
        gameManager.instance.youWin();
        Destroy(gameObject);
    }

    void faceTarget()
    {
        Vector3 directionToLook = new Vector3(PlayerPos.position.x, PlayerPos.position.y - 3f, PlayerPos.position.z) - transform.position;
        Quaternion rot = Quaternion.LookRotation(directionToLook);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 20f);
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

        if (Health <= 0)
        {
            StartCoroutine(MechDeath());
        }
    }
}
