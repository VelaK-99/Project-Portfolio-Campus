using UnityEngine;

public class Landmine : MonoBehaviour
{
    [SerializeField] Renderer model;
    [SerializeField] GameObject explosionEffect;
    [SerializeField] Color armedColor = Color.red;

    Color originalColor;
    bool isArmed = false;

    void Start()
    {
        originalColor = model.material.color;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isArmed)
        {
            isArmed = true;
            model.material.color = armedColor;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isArmed)
        {
            Explode();
        }
    }

    void Explode()
    {
        if (explosionEffect)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
