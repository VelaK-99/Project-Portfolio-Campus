using UnityEngine;
using System.Collections;

public class  turretTrap : MonoBehaviour
{
  
    [SerializeField] int faceTargetSpeed;

    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] float detectionRange;
    [SerializeField] AudioSource TurretSource;
    [SerializeField] AudioClip[] TurretSounds;
    [Range(0f, 2f)]  public float turretVol;

    float shootTimer;
    Vector3 playerDir;

    void Start()
    {
        
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, gameManager.instance.player.transform.position);

        if (distanceToPlayer <= detectionRange)
        {
            playerDir = (gameManager.instance.player.transform.position - transform.position);
            faceTarget();

            shootTimer += Time.deltaTime;
            if (shootTimer >= shootRate)
            {
                shoot();
            }
        }
    }

   

    void shoot()
    {
        shootTimer = 0;

        Vector3 dirToPlayer = (gameManager.instance.player.transform.position - shootPos.position).normalized;
        Quaternion bulletRot = Quaternion.LookRotation(dirToPlayer);

        Instantiate(bullet, shootPos.position, bulletRot);
        TurretSource.PlayOneShot(TurretSounds[Random.Range(0, TurretSounds.Length)], turretVol);
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }
}
