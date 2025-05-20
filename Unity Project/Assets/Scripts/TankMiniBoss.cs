using UnityEngine;

public class TankMiniBoss : MonoBehaviour, IDamage
{
    [SerializeField] int health;
    [SerializeField] GameObject deathExplosion;
    [SerializeField] GameObject turretToDestroy;

    public void TakeDamage(int dmg)
    {
        health -= dmg;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (deathExplosion != null)
        {
            Instantiate(deathExplosion, transform.position, Quaternion.identity);
        }

        if (turretToDestroy != null)
        {
            Destroy(turretToDestroy);
        }

        Destroy(gameObject);
    }
}
