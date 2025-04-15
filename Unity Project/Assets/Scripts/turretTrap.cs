using UnityEngine;

public class turretTrap : MonoBehaviour
{
    public Transform head; 
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate; 
    public float detectionRange;
    public float rotationSpeed;

    private Transform player;
    private float fireCooldown;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        fireCooldown = 0f;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= detectionRange)
        {
            AimAtPlayer();

           
                Shoot();
                fireCooldown = 1f / fireRate;
            
        }
    }

    void AimAtPlayer()
    {
        Vector3 direction = (player.position - head.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        head.rotation = Quaternion.Slerp(head.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    void Shoot()
    {
        fireCooldown = 0;
        Instantiate(projectilePrefab, firePoint.position, transform.rotation);
    }
}
