using UnityEngine;

public class TankShell : MonoBehaviour
{
    [SerializeField] float mSpeed = 20f;
    [SerializeField] GameObject mExplosionPrefab;
    [SerializeField] float mLifeTime = 5f;

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = transform.forward * mSpeed;
        }

        Destroy(gameObject, mLifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (mExplosionPrefab != null)
        {
            Instantiate(mExplosionPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
