using UnityEngine;
using System.Collections;
using UnityEngine.AI;


public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform headPos;

    public Spawner whereICameFrom;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int fov;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;

    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] int shootFOV;    

    float shootTimer;
    bool playerInRange;
    float roamTimer;
    float angleToPlayer;
    float stoppingDistOrig;

    

    Color colorOriginal;

    Vector3 playerDir;
    Vector3 startingPos;


    void Start()
    {
        colorOriginal = model.material.color;
        gameManager.instance.UpdateGameGoal(1);
        startingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;
    }

    void Update()
    {
        if (agent.remainingDistance < 0.01f)
            roamTimer += Time.deltaTime;

        if (playerInRange && !CanSeePlayer())
        {
            checkRoam();
        }
        else if (!playerInRange)
        {
            checkRoam();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            agent.stoppingDistance = 0;
        }
    }
    public void TakeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashRed());
        agent.SetDestination(gameManager.instance.player.transform.position);

        if (HP <= 0)
        {
            gameManager.instance.UpdateGameGoal(-1);
            whereICameFrom.spawnList.Remove(gameObject);
            whereICameFrom.GetComponent<Spawner>().checkEnemyTotal();
            Destroy(gameObject);
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOriginal;
    }

    void shoot()
    {
        shootTimer = 0;
        Instantiate(bullet, shootPos.position, transform.rotation);
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    void checkRoam()
    {
        if (roamTimer >= roamPauseTime && agent.remainingDistance < 0.01f)
        {
            roam();
        }
    }
    void roam()
    {
        roamTimer = 0;
        agent.stoppingDistance = 0;

        Vector3 randomPos = Random.insideUnitSphere * roamDist;
        randomPos += startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomPos, out hit, roamDist, 1);
        agent.SetDestination(hit.position);
    }

    bool CanSeePlayer()
    {
        playerDir = (gameManager.instance.player.transform.position - headPos.position);
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);
        Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= fov)
            {
                agent.SetDestination(gameManager.instance.player.transform.position);

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget();
                }

                shootTimer += Time.deltaTime;

                if (angleToPlayer <= shootFOV && shootTimer >= shootRate)
                {
                    shoot();
                }
                agent.stoppingDistance = stoppingDistOrig;
                return true;
            }
        }
        agent.stoppingDistance = 0;
        return false;
    }
}