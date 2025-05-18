using UnityEngine;

public class CarScript : MonoBehaviour
{
    [Header("===== Stats =====")]
    [SerializeField] int speed;
    [SerializeField] int acceleration;
    [SerializeField] int weight;
    [SerializeField] int handling;
    [SerializeField] int maxSpeed;
    [SerializeField] int minSpeed;
    [SerializeField] int brakingPower;
    [SerializeField] int CarDamage;


    Vector3 carDir;
    Vector3 carPos;
    Vector3 carRot;
    Vector3 carVelocioty;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
