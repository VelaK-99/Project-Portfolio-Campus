using UnityEngine;

public class ElectricGun : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] int damage = 10;
    [SerializeField] float lifeTime = 5f;
    [SerializeField] LayerMask hitLayer; 

    void Start()
    {
        Destroy(gameObject, lifeTime); 
        Debug.Log($"Electric Orb Spawned. Layer: {gameObject.layer}");
    }

    void Update()
    {        
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("EnemyBody"))
        {
            IDamage target = other.GetComponentInParent<IDamage>();

            if (target != null)
            {
                target.TakeDamage(damage);
            }
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
