using UnityEngine;
using System.Collections;
using UnityEngine.AI;


public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    [SerializeField] Transform headPos;
    [SerializeField] AudioSource aud;

    [Header("===== Stats =====")]
    [Range(0,100)] [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [Range(0,180)][SerializeField] int fov;
    [SerializeField] int animTranSpeed;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;
    [Range(0,25)] [SerializeField] float shootRate;
    [Range(0,45)] [SerializeField] int shootFOV; 

    [Header("===== Audio =====")]
    [SerializeField] AudioClip[] audShoot;
    [Range(0, 100)] [SerializeField] float audShootVol;
    [SerializeField] AudioClip[] audHurt;
    [Range(0, 100)] [SerializeField] float audHurtVol;
    [SerializeField] AudioClip[] audStep;
    [Range(0, 100)] [SerializeField] float audStepVol;

    [SerializeField] Collider knife;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;

    float shootTimer;
    bool playerInRange;
    float roamTimer;
    float angleToPlayer;
    float stoppingDistOrig;
    bool isPlayingStep;

    public Spawner whereICameFrom;


    Color colorOriginal;

    Vector3 playerDir;
    Vector3 startingPos;


    void Start()
    {
        colorOriginal = model.material.color;        
        startingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;
    }

    void Update()
    {
        onAnimLomotion();
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
    void onAnimLomotion()
    {
        float agentSpeedCur = agent.velocity.normalized.magnitude;
        float animSpeedCur = animator.GetFloat("speed");

        animator.SetFloat("speed", Mathf.Lerp(animSpeedCur, agentSpeedCur, Time.deltaTime * animTranSpeed));
        bool isMoving = agent.velocity.magnitude > 0.1f && agent.remainingDistance > agent.stoppingDistance;

        if (agent.velocity.magnitude > 0.1f && agent.remainingDistance > agent.stoppingDistance && !isPlayingStep)
        {
            StartCoroutine(playStep());
        }
    }

    IEnumerator playStep()
    {
        isPlayingStep = true;

        if (audStep.Length > 0)
        {
            aud.PlayOneShot(audStep[Random.Range(0, audStep.Length)], audStepVol);
        }

        yield return new WaitForSeconds(0.3f);

        isPlayingStep = false;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
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
        aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);
        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            if(whereICameFrom != null )
            {
                whereICameFrom.spawnList.Remove(gameObject);
                whereICameFrom.spawnList.RemoveAll(e => e == null);
                whereICameFrom.GetComponent<Spawner>().checkEnemyTotal();
            }
            else
            {
                Debug.LogWarning($"{name} has no spawner reference! Cannot update spawnList or unlock doors.");
            }
            
            
            
            gameManager.instance.RemoveEnemy(gameObject);

            
            Destroy(gameObject);
        }

        else
            agent.SetDestination(gameManager.instance.player.transform.position);
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOriginal;
    }

    void shoot()
    {
        aud.PlayOneShot(audShoot[Random.Range(0, audShoot.Length)], audShootVol);
        shootTimer = 0;
        animator.SetTrigger("shoot");
    }
    public void createBullet()
    {
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

    public void SwordColOn()
    {
        knife.enabled = true;
    }

    public void SwordColOff()
    {
        knife.enabled = false;
    }
}