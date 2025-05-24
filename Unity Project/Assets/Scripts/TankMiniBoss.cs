using UnityEngine;
using System.Collections;

public class TankMiniBoss : MonoBehaviour, IDamage
{
    [SerializeField] int health;
    [SerializeField] GameObject deathExplosion;
    [SerializeField] GameObject turretToDestroy;
    [SerializeField] Renderer flashRenderer; 
    [SerializeField] Color flashColor = Color.red;
    [SerializeField] float flashDuration = 0.1f;

    Color originalColor;

    void Start()
    {
        if (flashRenderer != null)
        {
            originalColor = flashRenderer.material.color;
        }
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;

        if (flashRenderer != null)
        {
            StartCoroutine(FlashRed());
        }

        if (health <= 0)
        {
            Die();
        }
    }

    IEnumerator FlashRed()
    {
        flashRenderer.material.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        flashRenderer.material.color = originalColor;
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