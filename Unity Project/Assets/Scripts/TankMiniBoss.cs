using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TankMiniBoss : MonoBehaviour, IDamage
{
    [SerializeField] int health;
    [SerializeField] GameObject deathExplosion;
    [SerializeField] GameObject turretToDestroy;
    [SerializeField] Color flashColor = Color.red;
    [SerializeField] float flashDuration = 0.1f;
    [SerializeField] gameManager gameManager;

    Renderer[] renderers;
    List<Color> originalColors = new List<Color>();

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();

        foreach (var rend in renderers)
        {
            originalColors.Add(rend.material.color);
        }
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;

        StartCoroutine(FlashRed());

        if (health <= 0)
        {
            Die();
        }
    }

    IEnumerator FlashRed()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = flashColor;
        }

        yield return new WaitForSeconds(flashDuration);

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = originalColors[i];
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

        if (gameManager != null)
        {
            gameManager.youWin();
        }

        Destroy(gameObject);
    }
}
 