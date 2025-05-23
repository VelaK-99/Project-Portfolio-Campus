using UnityEngine;

public class DestructibleWall : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DestroyWall(false);
    }

    public void DestroyWall( bool destructible)
    {
        foreach(Transform child in transform)
        {
            Rigidbody rb = child.GetComponent<Rigidbody>();
            if(rb != null )
            {
                rb.isKinematic = !destructible;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            DestroyWall(true);
        }
    }
}
