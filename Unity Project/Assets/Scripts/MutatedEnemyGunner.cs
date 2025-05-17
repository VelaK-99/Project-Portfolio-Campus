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
    [SerializeField] Transform[] patrolPoints;

    enum State { Patrolling, Chasing, Shooting, Retreating, Dead }
    State mCurrentState;

    NavMeshAgent mAgent;
    float mNextFireTime;
    int mCurrentPatrolIndex;

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
            RetreatFromPlayer();
            return;
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
        if (patrolPoints.Length == 0) return;

        if (!mAgent.pathPending && mAgent.remainingDistance < 0.5f)
        {
            GoToNextPatrolPoint();
        }
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        mAgent.destination = patrolPoints[mCurrentPatrolIndex].position;
        mCurrentPatrolIndex = (mCurrentPatrolIndex + 1) % patrolPoints.Length;
        animator.SetBool("isMoving", true);
    }

    IEnumerator FireBurst()
    {
        animator.SetBool("isShooting", true);

        for (int i = 0; i < burstCount; i++)
        {
            Vector3 spread = firePoint.forward;
            spread += firePoint.right * Random.Range(-0.1f, 0.1f);
            spread += firePoint.up * Random.Range(-0.1f, 0.1f);

            Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(spread));
            yield return new WaitForSeconds(burstDelay);
        }

        animator.SetBool("isShooting", false);
    }

    void RetreatFromPlayer()
    {
        Vector3 direction = (transform.position - player.position).normalized;
        Vector3 retreatPosition = transform.position + direction * retreatDistance;

        mAgent.SetDestination(retreatPosition);
        mCurrentState = State.Retreating;
        animator.SetBool("isMoving", true);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
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
}
