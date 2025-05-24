using UnityEngine;
using UnityEngine.AI;

public class EnemyExploder : MonoBehaviour, IDamage
{
    [SerializeField] int health;
    [SerializeField] float explosionRadius = 5f;
    [SerializeField] int explosionDamage = 30;
    [SerializeField] GameObject explosionEffect;
    [SerializeField] float explodeDistance = 2f; 

    Transform player;
    NavMeshAgent agent;

    void Start()
    {
        player = gameManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (!player) return;

        // Move toward the player
        agent.SetDestination(player.position);

        // Check if close enough to explode
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= explodeDistance)
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
