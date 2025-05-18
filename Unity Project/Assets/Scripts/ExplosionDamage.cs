using UnityEngine;

public class ExplosionDamage : MonoBehaviour
{
    [SerializeField] int damage = 20;
    [SerializeField] float radius = 5f;

    void Start()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                IDamage damageable = hit.GetComponent<IDamage>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damage);
                }
            }
        }

        Destroy(gameObject, 0.1f); // remove explosion after a short time
    }
}
