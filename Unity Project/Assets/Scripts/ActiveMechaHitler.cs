using UnityEngine;

public class ActiveMechaHitler : MonoBehaviour
{
   [SerializeField] mechaHitlerAI mecha;


   

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mecha.IsActive = true;
        }
    }
}
