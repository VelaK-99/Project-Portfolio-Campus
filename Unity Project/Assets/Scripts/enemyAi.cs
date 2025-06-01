using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage, IElectricJolt, IFrozen
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    [SerializeField] Transform headPos;
    [SerializeField] AudioSource aud;

    [Header("===== Stats =====")]
    [Range(0, 100)][SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [Range(0, 180)][SerializeField] int fov;
    [SerializeField] private int animTranSpeed;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;
    [Range(0, 25)][SerializeField] float shootRate;
    [Range(0, 45)][SerializeField] int shootFOV;
    public float stunTimer;
    [SerializeField] bool isMelee;

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

    [Header("===== Mob Drops =====")]
    [SerializeField] GameObject ammoDrop;
    [SerializeField] GameObject healthDrop;
    [SerializeField] float dropChance = 0.6f;

    private Transform currentCoverPoint;
    private float coverSwitchTimer;

    [SerializeField] Collider knife;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    public LineRenderer joltLine;

    float shootTimer;
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

    float smoothANGLE = 0f;


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
        onAnimLocomotion();

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

            EngagePlayer();
        }

    }


    void EngagePlayer()
    {

        if (CanSeePlayer())
        {
            agent.SetDestination(gameManager.instance.player.transform.position);


            UpdateAimingSystem();

            shootTimer += Time.deltaTime;
            if (shootTimer >= shootRate)
            {
                if (isMelee && agent.remainingDistance <= agent.stoppingDistance)
                {
                    shoot();
                }
                else if (!isMelee)
                {
                    shoot();
                }
            }
        }
        else
        {
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

                isTakingCover = false;
                return;
            }

            agent.SetDestination(currentCoverPoint.position);
        }

        UpdateAimingSystem();

        shootTimer += Time.deltaTime;
        if (shootTimer >= shootRate)
        {
            shoot();
        }

        if (Vector3.Distance(transform.position, currentCoverPoint.position) <= 0.5f)
        {
            currentCoverState = CoverState.AtCover;
            coverSwitchTimer = 0;
        }
    }

    void StayAtCover()
    {
        coverSwitchTimer += Time.deltaTime;

        if (!CanSeePlayer())
        {
            currentCoverState = CoverState.SwitchingCover;
            return;
        }

        UpdateAimingSystem();
        shootTimer += Time.deltaTime;
        if (shootTimer >= shootRate)
        {
            shoot();
        }

        if (coverSwitchTimer >= coverSwitchDelay)
        {
            currentCoverState = CoverState.SwitchingCover;
        }
    }

    void SwitchCover()
    {
        currentCoverPoint = GetRandomCoverPoint();
        if (currentCoverPoint == null)
        {
            isTakingCover = false;
            return;
        }

        agent.SetDestination(currentCoverPoint.position);
        currentCoverState = CoverState.MovingToCover;
    }

    Transform GetRandomCoverPoint()
    {
        if (coverPoints.Count == 0)
        {
            return null;
        }

        Transform selectedCover = coverPoints[Random.Range(0, coverPoints.Count)];
        coverPoints.Add(selectedCover);
        return selectedCover;
    }


    void onAnimLocomotion()
    {
        float agentSpeedCur = agent.velocity.normalized.magnitude;
        float animSpeedCur = animator.GetFloat("speed");

        animator.SetFloat("speed", Mathf.Lerp(animSpeedCur, agentSpeedCur, Time.deltaTime * animTranSpeed));
        bool isMoving = agent.velocity.magnitude > 0.1f && agent.remainingDistance > agent.stoppingDistance;

        if (isMoving && !isPlayingStep)
        {
            StartCoroutine(playStep());
        }

        /*
            if (agent.velocity.magnitude > 0.1f && agent.remainingDistance > agent.stoppingDistance && !isPlayingStep)
            {
                StartCoroutine(playStep());
            }
        */
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
            //playerInRange = true;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //playerInRange = false;
            if (!isMelee)
            {
                agent.stoppingDistance = 0;
            }
        }
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashRed());

        aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);


        if (HP <= 0)
        {
            DropPickup();
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
                }
            }
            else
            {
                isTakingCover = false;
            }
        }
    }

    public void Stun(float duration, Vector3 force)
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
        StartCoroutine(StopKnockback());
    }

    IEnumerator StopKnockback()
    {
        yield return new WaitForSeconds(0.3f); // Adjust duration as needed
        rb.linearVelocity = Vector3.zero; // Instantly stop movement
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

        if (agent != null)
        {
            agent.isStopped = false;
        }
        if (animator != null)
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
        if (!isMelee)
        {
            Vector3 shootDirection = shootPos.forward;
            
            /*
            Vector3 playerPosition = gameManager.instance.player.transform.position;
            Vector3 shootDirection = GetLookDirectionToPlayer(shootPos, 1.5f);
            */




            GameObject spawnedBullet = Instantiate(bullet, shootPos.position, Quaternion.LookRotation(shootDirection));
        }
    }

    
    Vector3 GetLookDirectionToPlayer(Transform fromTransform)
    {
        Vector3 targetPOS = gameManager.instance.player.GetComponent<Collider>().bounds.center;
        return (targetPOS - fromTransform.position).normalized;
    }

    /*
    void faceTarget()
    {
        Vector3 playerDirection = GetLookDirectionToPlayer(transform);
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(playerDirection.x, 0, playerDirection.z));
        //Quaternion targetRotation = Quaternion.LookRotation(playerDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * faceTargetSpeed);
    }

    void SmoothAIM()
    {
        if (!isMelee)
        {
            Vector3 targetDIR = GetLookDirectionToPlayer(shootPos, 1.5f);
            Quaternion targetROT = Quaternion.LookRotation(targetDIR);

            if (Vector3.Angle(shootPos.forward, targetDIR) > 0.5f)
           { shootPos.rotation = Quaternion.Lerp(shootPos.rotation, targetROT, Time.deltaTime * faceTargetSpeed); }
        }
    }

    void UpdateAIMangle()
    {
        

        if (!isMelee)
        {
            Vector3 DIR = GetLookDirectionToPlayer(shootPos, 1.5f);

            //signed angle based on right-axis (to isolate vertical angle)
            float ANGLE = Vector3.SignedAngle(shootPos.forward, DIR.normalized, transform.right);

           smoothANGLE = Mathf.Lerp(smoothANGLE, ANGLE, Time.deltaTime * 10f);

            smoothANGLE = Mathf.Lerp(smoothANGLE, -45f, 45f);

            float normalizedANGLE = Mathf.InverseLerp(-45f, 45f, smoothANGLE);

            animator.SetFloat("Angle", normalizedANGLE);
        }
    }
    */

    void UpdateAimingSystem()
    {
        if (isMelee) return;

        // ==Rotate ShootPOS==
        Vector3 aimDirection = GetLookDirectionToPlayer(shootPos); //includes vertical offset

        // ==Rotate Body ==
        Vector3 flatDirection = new Vector3(aimDirection.x, 0f, aimDirection.z); //Y removed
        //Quaternion bodyRotation = Quaternion.LookRotation(new Vector3(flatDirection.x, 0f, flatDirection.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(flatDirection), Time.deltaTime * faceTargetSpeed);

        //Vertical + horizontal aim
        Quaternion aimRotation = Quaternion.LookRotation(aimDirection);
        shootPos.rotation = Quaternion.Lerp(shootPos.rotation, aimRotation, Time.deltaTime * faceTargetSpeed);
        
        /*
        if (Vector3.Angle(shootPos.forward, aimDirection) > 0.5f)
        {
            shootPos.rotation = Quaternion.Lerp(shootPos.rotation, shootRotation, Time.deltaTime * faceTargetSpeed);
        }
        */

        // ==UPDATE AIM ANIMATION
        float rawAngle = Vector3.SignedAngle(shootPos.forward, aimDirection, transform.right);
        smoothANGLE = Mathf.Lerp(smoothANGLE, rawAngle, Time.deltaTime * 10f);
        smoothANGLE = Mathf.Clamp(smoothANGLE, -45f, 45f);
        float normalizedAngle = Mathf.InverseLerp(-45f, 45f, smoothANGLE);
        animator.SetFloat("Angle", normalizedAngle);

#if UNITY_EDITOR
        Debug.DrawRay(transform.position + Vector3.up * 1.5f, transform.forward * 5f, Color.blue);       // Body forward
        Debug.DrawRay(shootPos.position, shootPos.forward * 5f, Color.red);                             // Gun forward
        Debug.DrawLine(shootPos.position, gameManager.instance.player.transform.position, Color.green); // Line to player
#endif

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
            return false;
        }

        playerDir = (gameManager.instance.player.transform.position - headPos.position).normalized;
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);

        if (angleToPlayer > fov)
        {
            return false;
        }

        Vector3 rayOrigin = headPos.position;
        Vector3 targetPoint = gameManager.instance.player.transform.position + Vector3.up * 1.5f;
        Debug.DrawRay(rayOrigin, (targetPoint - rayOrigin).normalized * 50f, Color.red);

        RaycastHit hit;
        int layerMask = ~(1 << LayerMask.NameToLayer("Ground")); // Ignore ground

        if (Physics.Raycast(rayOrigin, (targetPoint - rayOrigin).normalized, out hit, 50f, layerMask))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }
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
            if (joltLine) joltLine.SetPosition(0, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z));
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
                StartCoroutine(DelayJolt(closestEnemy, joltAmount, joltChainLength));
                StartCoroutine(ResetJolt());
            }
        }
    }

    IEnumerator DelayJolt(GameObject closestEnemy, int joltAmount, int joltChainLength)
    {
        yield return new WaitForSeconds(0.2f);
        if (closestEnemy != null)
        {
            if (joltLine) joltLine.SetPosition(1, new Vector3(closestEnemy.transform.position.x, closestEnemy.transform.position.y + 1, closestEnemy.transform.position.z));
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

    IEnumerator ResetJolt()
    {
        yield return new WaitForSeconds(2f);
        hasBeenJolted = false;
    }

    IEnumerator ShowJolt()
    {
        joltLine.enabled = true;
        yield return new WaitForSeconds(0.05f);
        joltLine.enabled = false;
    }

    void DropPickup()
    {
        if (Random.value < dropChance)
        {
            int dropType = Random.Range(0, 2);

            if (dropType == 0 && ammoDrop != null)
                Instantiate(ammoDrop, transform.position, Quaternion.identity);
            else if (dropType == 1 && healthDrop != null)
                Instantiate(healthDrop, transform.position, Quaternion.identity);
        }
    }



    void Movement()
    {
        //leave empty
    }

    public void FrozenVisual(int _freezeDuration)
    {
        StartCoroutine(flashBlue(_freezeDuration));
    }

    IEnumerator flashBlue(int _freezeDuration)
    {
        model.material.color = Color.blue;
        yield return new WaitForSeconds(_freezeDuration);
        model.material.color = colorOriginal;
    }
    
    void NewEvent()
    {
        //leave empty
    }
}
