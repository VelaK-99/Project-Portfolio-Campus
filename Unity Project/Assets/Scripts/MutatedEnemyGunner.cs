using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MutatedEnemyGunner : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float detectionRange;
    [SerializeField] float fireRate;
    [SerializeField] float moveSpeed;
    [SerializeField] int health;
    [SerializeField] Animator animator;
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audShoot;
    [Range(0, 100)][SerializeField] float audShootVol;
    [SerializeField] AudioClip[] audHurt;
    [Range(0, 100)][SerializeField] float audHurtVol;
    [SerializeField] AudioClip[] audStep;
    [Range(0, 100)][SerializeField] float audStepVol;

    NavMeshAgent mAgent;
    float mNextFireTime;
    bool isPlayingStep;

    void Start()
    {
        mAgent = GetComponent<NavMeshAgent>();
        mAgent.speed = moveSpeed;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            mAgent.SetDestination(player.position);
            animator.SetBool("isMoving", true);

            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;
            transform.forward = direction;

            if (Time.time >= mNextFireTime)
            {
                Shoot();
                mNextFireTime = Time.time + 1f / fireRate;
            }

            animator.SetBool("isShooting", true);


            if (mAgent.velocity.magnitude > 0.1f && mAgent.remainingDistance > mAgent.stoppingDistance && !isPlayingStep)
            {
                StartCoroutine(playStep());
            }
        }
        else
        {
            mAgent.ResetPath();
            animator.SetBool("isMoving", false);
            animator.SetBool("isShooting", false);
        }
    }

    void Shoot()
    {
        aud.PlayOneShot(audShoot[Random.Range(0, audShoot.Length)], audShootVol);
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        animator.SetTrigger("die");
        mAgent.enabled = false;
        this.enabled = false;
    }

    IEnumerator playStep()
    {
        isPlayingStep = true;

        if (audStep.Length > 0)
        {
            aud.PlayOneShot(audStep[Random.Range(0, audStep.Length)], audStepVol);
        }

        yield return new WaitForSeconds(0.3f);

        isPlayingStep = false;
    }
}