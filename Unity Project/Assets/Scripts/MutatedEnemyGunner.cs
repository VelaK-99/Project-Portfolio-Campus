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

    NavMeshAgent mAgent;
    float mNextFireTime;

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
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
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
        animator.SetTrigger("die");
        mAgent.enabled = false;
        this.enabled = false;
    }
}