using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MutatedEnemyGunner : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float detectionRange;
    [SerializeField] float fireRate;
    [SerializeField] float moveSpeed;
    [SerializeField] int health;
    [SerializeField] int lowHealthThreshold;
    [SerializeField] float retreatDistance;
    [SerializeField] int burstCount;
    [SerializeField] float burstDelay;
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
        mCurrentState = State.Patrolling;
        GoToNextPatrolPoint();
    }

    void Update()
    {
        if (mCurrentState == State.Dead || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (health <= lowHealthThreshold)
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

        switch (mCurrentState)
        {
            case State.Patrolling:
                Patrol();
                if (distance <= detectionRange)
                    mCurrentState = State.Chasing;
                break;

            case State.Chasing:
                if (distance > detectionRange)
                {
                    mCurrentState = State.Patrolling;
                    animator.SetBool("isMoving", true);
                    GoToNextPatrolPoint();
                    return;
                }

                mAgent.SetDestination(player.position);
                animator.SetBool("isMoving", true);

                if (distance <= mAgent.stoppingDistance + 2f)
                {
                    mCurrentState = State.Shooting;
                    animator.SetBool("isMoving", false);
                }
                break;

            case State.Shooting:
                transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

                if (Time.time >= mNextFireTime)
                {
                    StartCoroutine(FireBurst());
                    mNextFireTime = Time.time + 1f / fireRate;
                }

                if (distance > detectionRange)
                {
                    mCurrentState = State.Chasing;
                }
                break;

            case State.Retreating:
                RetreatFromPlayer();
                break;
        }
    }

    void Patrol()
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
        mCurrentState = State.Dead;
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
