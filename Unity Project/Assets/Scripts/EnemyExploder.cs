using UnityEngine;
using UnityEngine.AI;

public class EnemyExploder : MonoBehaviour, IDamage
{
    [SerializeField] int health;
    [SerializeField] float explosionRadius;
    [SerializeField] int explosionDamage;
    [SerializeField] GameObject explosionEffect;
    [SerializeField] float moveSpeed;

    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
    }

    void Update()
    {
        if (!gameManager.instance.player) return;

        agent.SetDestination(gameManager.instance.player.transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Explode();
        }
    }

    void Explode()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hit in hitColliders)
        {
            IDamage target = hit.GetComponent<IDamage>();
            if (target != null)
            {
                target.TakeDamage(explosionDamage);
            }
        }

        Destroy(gameObject);
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            Explode();
        }
    }
}
