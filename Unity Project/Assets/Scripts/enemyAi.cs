using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage, IElectricJolt
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    [SerializeField] Transform headPos;
    [SerializeField] public Transform torsoPos;
    [SerializeField] AudioSource aud;

    [Header("===== Stats =====")]
    [Range(0, 100)][SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [Range(0, 180)][SerializeField] int fov;
    [SerializeField] int animTranSpeed;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;
    [Range(0, 25)][SerializeField] float shootRate;
    [Range(0, 45)][SerializeField] int shootFOV;    
    public float stunTimer;    

    [Header("===== Audio =====")]
    [SerializeField] AudioClip[] audShoot;
    [Range(0, 100)][SerializeField] float audShootVol;
    [SerializeField] AudioClip[] audHurt;
    [Range(0, 100)][SerializeField] float audHurtVol;
    [SerializeField] AudioClip[] audStep;
    [Range(0, 100)][SerializeField] float audStepVol;

    [Header("===== Cover System =====")]    
    [SerializeField] List<Transform> coverPoints;
    [SerializeField] float coverSwitchDelay = 2f;
    [SerializeField] bool useCoverSystem = true;
    [SerializeField] float peekDistance = 0.75f;
    [SerializeField] float peekSpeed = 5f;
    [SerializeField] float coverDetectionRadius = 20f;
    

    private Transform currentCoverPoint;
    private float coverSwitchTimer;

    [SerializeField] Collider knife;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    public LineRenderer joltLine;

    float shootTimer;
    bool playerInRange;
    float roamTimer;
    float angleToPlayer;
    float stoppingDistOrig;
    bool isPlayingStep;
    bool isTakingCover = false;
    bool isAtCover;
    bool isStuned;
    bool isFrozen = false;
    float freezeTimer = 0f;
    Rigidbody rb;

    private enum CoverState { MovingToCover, AtCover, SwitchingCover }
    private CoverState currentCoverState = CoverState.MovingToCover;
    

    [SerializeField] float coverDamageCooldown = 1.5f;
    private float lastDamageTime = -100f;
    public bool hasBeenJolted;

    Color colorOriginal;

    Vector3 playerDir;
    Vector3 startingPos;
    Vector3 coverPosition;
    Vector3 knockbackfoce;


    void Start()
    {
        colorOriginal = model.material.color;
        startingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    void Update()
    {
        if (isFrozen)
        {
            freezeTimer -= Time.deltaTime;
            if (freezeTimer <= 0f)
            {
                Unfreeze();
            }
            return;
        }

        if (agent.pathStatus == NavMeshPathStatus.PathComplete)
            if (isStuned)
            {
                stunTimer -= Time.deltaTime;


                rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.deltaTime * 5f);

                if (stunTimer <= 0f)
                {
                    isStuned = false;
                    rb.linearVelocity = Vector3.zero;
                    rb.isKinematic = true;
                    agent.isStopped = false;
                }

                return;
            }

        if (useCoverSystem && coverPoints.Count > 0)
        {
            if (isTakingCover)
            {
                HandleCoverBehavior();
            }
            else
            {
                EngagePlayer();
            }
        }
        else
        {
            // No cover system - just engage player directly
            EngagePlayer();
        }

        onAnimLocomotion();
    }


    void EngagePlayer()
    {
        // If the player is within range and the enemy can see them
        if (CanSeePlayer())
        {
            // Move towards the player
            agent.SetDestination(gameManager.instance.player.transform.position);

            // Face the player
            faceTarget();

            // Shoot at the player if within shooting range
            shootTimer += Time.deltaTime;
            if (shootTimer >= shootRate)
            {
                shoot();
            }
        }
        else
        {
            // If the player is not visible, the enemy roams
            checkRoam();
        }
    }

    void HandleCoverBehavior()
    {
        switch (currentCoverState)
        {
            case CoverState.MovingToCover:
                MoveToCover();
                break;

            case CoverState.AtCover:
                StayAtCover();
                break;

            case CoverState.SwitchingCover:
                SwitchCover();
                break;
        }
    }

    void MoveToCover()
    {
        if (currentCoverPoint == null)
        {
            currentCoverPoint = GetRandomCoverPoint();
            if (currentCoverPoint == null)
            {
                Debug.LogWarning("No Available cover. staying in place");

                isTakingCover = false;
                return;
            }

            agent.SetDestination(currentCoverPoint.position);
            Debug.Log($"Moving to cover point: {currentCoverPoint.name} at {currentCoverPoint.position}");
        }

        faceTarget();

        shootTimer += Time.deltaTime;
        if (shootTimer >= shootRate)
        {
            shoot();
        }

        if (Vector3.Distance(transform.position, currentCoverPoint.position) <= 0.5f)
        {
            Debug.Log("Enemy reached cover.");
            currentCoverState = CoverState.AtCover;
            coverSwitchTimer = 0;
        }
    }

    void StayAtCover()
    {
        coverSwitchTimer += Time.deltaTime;

        if (!CanSeePlayer())
        {
            Debug.Log("Can't see player, switching cover.");
            currentCoverState = CoverState.SwitchingCover;
            return;
        }

        faceTarget();
        shootTimer += Time.deltaTime;
        if (shootTimer >= shootRate)
        {
            shoot();
        }

        if (coverSwitchTimer >= coverSwitchDelay)
        {
            currentCoverState = CoverState.SwitchingCover;
            Debug.Log("Switching cover after delay");
        }
    }

    void SwitchCover()
    {        
        currentCoverPoint = GetRandomCoverPoint();
        if (currentCoverPoint == null)
        {
            Debug.LogWarning("No available cover to switch to.");
            isTakingCover = false;
            return;
        }

        agent.SetDestination(currentCoverPoint.position);
        currentCoverState = CoverState.MovingToCover;
        Debug.Log($"Switching to new cover point: {currentCoverPoint.name}");
    }

    Transform GetRandomCoverPoint()
    {
        if (coverPoints.Count == 0)
        {
            Debug.LogWarning("No cover points assigned.");
            return null;
        }   

        Transform selectedCover = coverPoints[Random.Range(0, coverPoints.Count)];
        coverPoints.Add(selectedCover);
        Debug.Log($"Selected Cover Point: {selectedCover.name} at {selectedCover.position}");
        return selectedCover;
    } 


        void onAnimLocomotion()
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
        StartCoroutine(flashRed());

        aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);

        if (HP <= 0)
        {
            Destroy(gameObject);
            return;
        }

        if (Time.time - lastDamageTime >= coverDamageCooldown)
        {
            lastDamageTime = Time.time;

            if (useCoverSystem && coverPoints.Count > 0)
            {
                isTakingCover = true;
                currentCoverPoint = GetRandomCoverPoint();

                if (currentCoverPoint != null)
                {
                    agent.SetDestination(currentCoverPoint.position);
                    currentCoverState = CoverState.MovingToCover;
                    Debug.Log($"Taking Cover: Moving to {currentCoverPoint.position}");
                }
                else
                {
                    Debug.LogWarning("No available cover. Staying exposed.");
                }
            }
            else
            {
                Debug.Log("Cover system disabled or no cover points. Engaging without cover.");
                isTakingCover = false;
            }
        }
    }

    public void Stun(float duration,Vector3 force)
    {
        if (isStuned)
        {
            return;
        }
        isStuned = true;
        stunTimer = duration;
        knockbackfoce = force;
        
        agent.isStopped = true;
        rb.isKinematic = false;
        rb.AddForce(force,ForceMode.Impulse);
    }

    public void ApplyFreeze(float duration)
    {
        if (isFrozen) return;

        isFrozen = true;
        freezeTimer = duration;

        if (agent != null)
        {
            agent.isStopped = true;
        }
        if (animator != null)
        {
            animator.enabled = false;
        }
    }

    private void Unfreeze()
    {
        isFrozen = false;

        if(agent != null)
        {
            agent.isStopped = false;
        }
        if(animator != null)
        {
            animator.enabled = true;
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
        aud.PlayOneShot(audShoot[Random.Range(0, audShoot.Length)], audShootVol);
        shootTimer = 0;
        animator.SetTrigger("shoot");
    }
    public void createBullet()
    {
        // Calculate direction to player but ignore Y-axis (horizontal only)
        Vector3 playerPosition = gameManager.instance.player.transform.position;
        Vector3 shootDirection = (new Vector3(playerPosition.x, shootPos.position.y, playerPosition.z) - shootPos.position).normalized;

        // Instantiate bullet and make it face the player
        GameObject spawnedBullet = Instantiate(bullet, shootPos.position, Quaternion.LookRotation(shootDirection));
    }



    void faceTarget()
        {
            Vector3 playerDirection = (gameManager.instance.player.transform.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(playerDirection.x, 0, playerDirection.z));
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * faceTargetSpeed);
        }

        void checkRoam()
        {
            roamTimer += Time.deltaTime;
            if (roamTimer >= roamPauseTime)
            {
                roamTimer = 0;
                roam();
            }
        }
        void roam()
        {
            Vector3 randomPos = Random.insideUnitSphere * roamDist + startingPos;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPos, out hit, roamDist, 1))
            {
                agent.SetDestination(hit.position);
            }
        }

        bool CanSeePlayer()
        {

        if (gameManager.instance.player == null)
        {
            Debug.Log("Player not found.");
            return false;
        }

        // Calculate direction to player
        playerDir = (gameManager.instance.player.transform.position - headPos.position).normalized;
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);
        
        if (angleToPlayer > fov)
        {
            Debug.Log("Player is outside of FOV.");
            return false;
        }
        
        Vector3 rayOrigin = headPos.position;
        Vector3 targetPoint = gameManager.instance.player.transform.position + Vector3.up * 1.5f;
        Debug.DrawRay(rayOrigin, (targetPoint - rayOrigin).normalized * 50f, Color.red);

        RaycastHit hit;
        int layerMask = ~(1 << LayerMask.NameToLayer("Ground")); // Ignore ground

        if (Physics.Raycast(rayOrigin, (targetPoint - rayOrigin).normalized, out hit, 50f, layerMask))
        {
            Debug.Log($"Raycast hit: {hit.collider.name}");
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("Player is in sight!");
                return true;
            }
            else
            {
                Debug.Log($"Raycast blocked by: {hit.collider.name}");
            }
        }

        Debug.Log("Player is not in sight.");
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


    public void JoltEffect(int joltAmount, int joltChainLength)
    {
        if (joltChainLength == 0)
        {
            StartCoroutine(ResetJolt());
            return;
        }
        else
        {
            if(joltLine) joltLine.SetPosition(0, torsoPos.position);
            GameObject closestEnemy = null;
            Collider[] hitColliders = Physics.OverlapSphere(headPos.position, 5);
            float shortestDistance = Mathf.Infinity;
            EnemyAI enemyCheck = null;
            foreach (var hit in hitColliders)
            {
                if (hit.CompareTag("Enemy"))
                {
                    enemyCheck = hit.GetComponent<EnemyAI>();
                    float distance = Vector3.Distance(headPos.position, hit.transform.position);
                    if (enemyCheck != null && distance < shortestDistance && !enemyCheck.hasBeenJolted)
                    {
                        shortestDistance = distance;
                        closestEnemy = hit.gameObject;
                    }
                }
            }
            StartCoroutine(DelayJolt(closestEnemy, joltAmount, joltChainLength));
            StartCoroutine(ResetJolt());
        }
    }

    IEnumerator ResetJolt()
    {
        yield return new WaitForSeconds(2f);
        hasBeenJolted = false;
    }

    IEnumerator DelayJolt(GameObject closestEnemy, int joltAmount, int joltChainLength)
    {
        yield return new WaitForSeconds(0.2f);
        if (closestEnemy != null)
        {
            if(joltLine) joltLine.SetPosition(1, closestEnemy.transform.position);
            StartCoroutine(ShowJolt());
            IDamage dmg = closestEnemy.GetComponent<IDamage>();
            if (dmg != null)
            {
                dmg.TakeDamage(joltAmount / 2);
            }
            IElectricJolt jolt = closestEnemy.GetComponent<IElectricJolt>();
            if (jolt != null)
            {
                EnemyAI enemyScript = closestEnemy.GetComponent<EnemyAI>();
                if (enemyScript != null)
                {
                    enemyScript.hasBeenJolted = true;
                }
                jolt.JoltEffect(joltAmount / 2, joltChainLength - 1);
            }
        }
    }
    IEnumerator ShowJolt()
    {
        joltLine.enabled = true;
        yield return new WaitForSeconds(0.05f);
        joltLine.enabled = false;
    }
}
