using UnityEngine;

public class ElectricGun : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] int damage = 10;
    [SerializeField] float lifeTime = 5f;
    [SerializeField] LayerMask hitLayer; // Set this to "EnemyBody" in the inspector

    void Start()
    {
        Destroy(gameObject, lifeTime); // Destroy orb after a certain time
        Debug.Log($"Electric Orb Spawned. Layer: {gameObject.layer}");
    }

    void Update()
    {
        // Move the orb forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log($"Electric Orb collided with: {other.name} on Layer: {other.gameObject.layer}");

        // Check if the other object is in the hitLayer (EnemyBody)
        if (other.gameObject.layer == LayerMask.NameToLayer("EnemyBody"))
        {
            Debug.Log($"Hit detected on EnemyBody layer: {other.name}");

            // Attempt to get the IDamage component from the parent (main enemy script)
            IDamage target = other.GetComponentInParent<IDamage>();

            if (target != null)
            {
                target.TakeDamage(damage);
                Debug.Log($"Enemy {other.transform.parent.name} hit for {damage} damage.");
            }
            else
            {
                Debug.Log("No IDamage script found on the target.");
            }
        }
        else
        {
            Debug.Log("Collision ignored, not EnemyBody layer.");
        }
    }

    public void SetDamage(int dmg)
    {
        damage = dmg;
    }

    public void SetLifetime(float life)
    {
    lifeTime = life; 
    }
}
