using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [Header("===== Components =====")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] Transform headPos;

    [Header("===== Stats =====")]
    [SerializeField] int XP;
    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int fov;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;
    [SerializeField] int animTransSpeed;

    [Header("===== Weapons =====")]
    [SerializeField] Transform shootPOS;
    [SerializeField] GameObject bullet;
    [SerializeField] int shootFOV;
    [SerializeField] float shootRate;

    public Spawner whereICameFrom;

    float shootTimer;
    float roamTimer;
    float angleToPlayer;
    float stoppingDistOrig;

    bool playerInRange;

    Color colorOrig;

    Vector3 playerDir;
    Vector3 startingPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;        
        startingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {

        getAnimLocomotion();

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

    void getAnimLocomotion()
    {
        float agentSpeed = agent.velocity.normalized.magnitude;
        float animSpeedCur = anim.GetFloat("Speed");

        anim.SetFloat("Speed", Mathf.Lerp(animSpeedCur, agentSpeed, Time.deltaTime * animTransSpeed));
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
        playerDir = (gameManager.instance.player.transform.position - headPos.position).normalized;
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);

        if (angleToPlayer > fov)
        {
            Debug.Log("Player is outside of FOV");
            return false;
        }

        Vector3 rayOrigin = headPos.position;
        Vector3 targetPoint = gameManager.instance.player.transform.position + Vector3.up * 1.5f;

        Debug.DrawRay(rayOrigin, (targetPoint - rayOrigin).normalized * 50f, Color.red);

       RaycastHit hit;
        int layerMask = ~(1 << LayerMask.NameToLayer("Ground"));

        if (Physics.Raycast(rayOrigin, (targetPoint - rayOrigin).normalized, out hit, 50f, layerMask)) ;
        {
            Debug.Log($"Raycast hit: {hit.collider.name}");

            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("Player is in sight!");
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
        Debug.Log("Player is not in sight.");
        agent.stoppingDistance = 0;
        return false;
    }

    private void OnTriggerStay(Collider other)
    {

        Debug.Log("Player detected in trigger area.");

        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
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
            Destroy(gameObject);
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    void shoot()
    {
        shootTimer = 0;
        anim.SetTrigger("shoot");
    }

    public void createBullet()
    {
        Instantiate(bullet, shootPOS.position, transform.rotation);
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }
}