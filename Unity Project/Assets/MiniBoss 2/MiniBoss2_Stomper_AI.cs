using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

public class MiniBoss2_Stomper_AI : MonoBehaviour, IDamage, IElectricJolt
{
    MiniBoss2_Stomper_AI instance;

    [Header("===== Model Info =====")]
    /// <summary>
    /// Used to tie in a model/mesh for the enemy AI script
    /// </summary>
    [SerializeField] Renderer MODEL;

    [SerializeField] Rigidbody rb;

    /// <summary>
    /// This is tied to NavMeshSurface to allow Unity's pathfinding
    /// </summary>
    [SerializeField] NavMeshAgent AGENT;

    /// <summary>
    /// Where the new raycast will come from
    /// </summary>
    [SerializeField] Transform headPOSITON;
    [SerializeField] public Transform torsoPos;

    /// <summary>
    /// The controller that is attached to the mesh
    /// </summary>
    [SerializeField] Animator anim;

    [SerializeField] float animTRANspeed;

    [SerializeField] public float HP;

    [Header("===== Enemy Stats =====")]

    [SerializeField] MiniBoss2_Shockwave ShockwaveScript;

    /// <summary>
    /// The position of where projectiles come from, generally attached to the hand
    /// </summary>
    [SerializeField] Transform shootPOS;
    /// <summary>
    /// The gameObject in question attached to the projectile
    /// </summary>
    [SerializeField] GameObject BULLET;
    [SerializeField] float shootRATE;
    /// <summary>
    /// How fast the enemy turns at the target, further defined in void faceTARGET().
    /// </summary>
    [SerializeField] float faceTARGETspeed;

    /// <summary>
    /// Enemy's field of view
    /// </summary>
    [SerializeField] int FOV;

    [SerializeField] int shootFOV;

    [Header("===== Roam Paramaters =====")]
    /// <summary>
    /// how far can the enemy roam in a perimeter
    /// </summary>
    [SerializeField] int roam_DISTANCE;
    /// <summary>
    /// how long will an enemy pause in a position
    /// </summary>
    [SerializeField] int roam_PAUSEtime;



    [Header("===== Cover System =====")]
    [SerializeField] List<Transform> coverPoints;
    [SerializeField] float coverSwitchDelay = 2f;
    [SerializeField] bool useCoverSystem = true;
    [SerializeField] float peekDistance = 0.75f;
    [SerializeField] float peekSpeed = 5f;
    [SerializeField] float coverDetectionRadius = 20f;

    public LineRenderer joltLine;
    bool isTakingCover = false;
    bool isAtCover;
    bool isStuned;
    bool isFrozen = false;
    public float stunTimer;
    float freezeTimer = 0f;

    private Transform currentCoverPoint;
    private float coverSwitchTimer;


    private enum CoverState { MovingToCover, AtCover, SwitchingCover }
    private CoverState currentCoverState = CoverState.MovingToCover;


    [SerializeField] float coverDamageCooldown = 1.5f;
    private float lastDamageTime = -100f;
    public bool hasBeenJolted;

    Vector3 knockbackfoce;

    [Header("===== Audio =====")]
    [SerializeField] AudioSource Audio;
    [SerializeField] AudioClip[] audShoot;
    [Range(0f, 2f)][SerializeField] float audShootVol;
    [SerializeField] AudioClip[] audHurt;
    [Range(0f, 2f)][SerializeField] float audHurtVol;
    [SerializeField] AudioClip[] audStep;
    [Range(0f, 2f)][SerializeField] float audStepVol;

    [Header("===== Next Scene Win =====")]
    [SerializeField] private string scenename;
    [SerializeField] MiniBoss2_Rager_AI twin;

    /// <summary>
    /// For melee collisions
    /// </summary>
    //[SerializeField] Collider SwordCOLLIDE;



    /// <summary>
    /// a check to see if the player in range is true
    /// </summary>
    bool player_IN_RANGE;

    /// <summary>
    /// Calculation variable for when to start roaming again
    /// </summary>
    float roamTIMER;

    /// <summary>
    /// To calculate where the enemy can see based on an angle
    /// </summary>
    float angleTO_PLAYER;

    /// <summary>
    /// any time roaming, stopping distance must be 0.
    /// </summary>
    float stoppingDistance_ORIGINAL;

    /// <summary>
    /// predicates the shoot rate. If shootTimer 1; shootRATE of 0.5 would be 2 shots a second
    /// </summary>
    float shootTIMER;

    bool isMoving;
    bool isPlayingStep;


    /*
    /// <summary>
    /// Creating a vector3 for the playerDIRECTION, further defined in void Update: 
    // playerDIRECTION = (Game_Management.INSTANCE.PLAYER.transform.position - transform.position);
    /// </summary>
    Vector3 playerDIRECTION;

    /// <summary>
    /// Where the enemy is within the worldspace
    /// </summary>
    Vector3 startingPOSITION;

    /// <summary>
    /// The original color of the mesh
    /// </summary>
    Color colorORIGINAL;
    */

    Color colorOriginal;

    Vector3 playerDir;
    Vector3 startingPos;
    Vector3 coverPosition;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {

        colorOriginal = MODEL.material.color;
        startingPos = transform.position;
        stoppingDistance_ORIGINAL = AGENT.stoppingDistance;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        /*
       // WINcondition(twin);

        colorORIGINAL = MODEL.material.color;

        //Left out to allow for the spawner to update the game goal
        //Game_Management.INSTANCE.updateGAMEgoal(1); //adding 1 to the gameGOALamount
        //referencing amount of enemies
       
        instance = this;

        startingPOSITION = transform.position;
        stoppingDistance_ORIGINAL = AGENT.stoppingDistance;


        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        */
    }

    // Update is called once per frame
    void Update()
    {
        animLOCOmotion();

        if (isFrozen)
        {
            freezeTimer -= Time.deltaTime;
            if (freezeTimer <= 0f)
            {
                Unfreeze();
            }
            return;
        }

        if (AGENT.pathStatus == NavMeshPathStatus.PathComplete)
            if (isStuned)
            {
                stunTimer -= Time.deltaTime;


                rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.deltaTime * 5f);

                if (stunTimer <= 0f)
                {
                    isStuned = false;
                    rb.linearVelocity = Vector3.zero;
                    rb.isKinematic = true;
                    AGENT.isStopped = false;
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

        //onAnimLocomotion();

        /*
        animLOCOmotion();



        if (AGENT.remainingDistance < 0.01f)
        {
            roamTIMER += Time.deltaTime;
        }

        if (player_IN_RANGE && !CANsee_PLAYER())
        {
            checkROAM();
        }
        else if (!player_IN_RANGE)
        {
            checkROAM();
        }
        */
    }

    void animLOCOmotion()
    {
        float AGENT_speedCUR = AGENT.velocity.normalized.magnitude;
        float anim_speedCUR = anim.GetFloat("Speed");

        anim.SetFloat("Speed", Mathf.Lerp(anim_speedCUR, AGENT_speedCUR, Time.deltaTime * animTRANspeed));
        bool isMoving = AGENT.velocity.magnitude > 0.1f && AGENT.remainingDistance > AGENT.stoppingDistance;

        if (isMoving && !isPlayingStep)
        {
            StartCoroutine(playStep());
        }
    }

    void checkRoam()
    {
        roamTIMER += Time.deltaTime;
        if (roamTIMER >= roam_PAUSEtime)
        {
            roamTIMER = 0;
            roam();
        }
    }

    void roam()
    {
        Vector3 randomPos = Random.insideUnitSphere * roam_DISTANCE + startingPos;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPos, out hit, roam_DISTANCE, 1))
        {
            AGENT.SetDestination(hit.position);
        }
    }

    /*
    void checkROAM()
    {
        if (roamTIMER >= roam_PAUSEtime && AGENT.remainingDistance <= 0.01f)
        {
            roam();
        }
    }

    void roam()
    {
        //Soon as he enters his destination, the timer should reset to 0
        roamTIMER = 0;
        AGENT.stoppingDistance = 0;

        ///selecting a random position based on the applied roam_DISTANCE
        Vector3 ranPOS = Random.insideUnitSphere * roam_DISTANCE;
        ranPOS += startingPos;

        ///To know exactly where the mesh is at, to go only on the map where a NavMesh is at
        NavMeshHit hit;

        ///Selecting a position in the Navmesh; the 1 is for the default layer, not using multiple navmesh layers
        NavMesh.SamplePosition(ranPOS, out hit, roam_DISTANCE, 1);

        //The location in which the NPC should go
        AGENT.SetDestination(hit.position);
    }
    */


    void EngagePlayer()
    {

        if (CanSeePlayer())
        {

            AGENT.SetDestination(gameManager.instance.player.transform.position);


            faceTARGET();
            ShockwaveScript.TryShockwave();

            shootTIMER += Time.deltaTime;
            if (shootTIMER >= shootRATE)
            {
                enemySHOOT();
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

            AGENT.SetDestination(currentCoverPoint.position);
        }

        faceTARGET();

        shootTIMER += Time.deltaTime;
        if (shootTIMER >= shootRATE)
        {
            enemySHOOT();
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

        faceTARGET();
        shootTIMER += Time.deltaTime;
        if (shootTIMER >= shootRATE)
        {
            enemySHOOT();
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

        AGENT.SetDestination(currentCoverPoint.position);
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


    bool CanSeePlayer()
    {

        if (gameManager.instance.player == null)
        {
            return false;
        }

        playerDir = (gameManager.instance.player.transform.position - headPOSITON.position).normalized;
        angleTO_PLAYER = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);

        if (angleTO_PLAYER > FOV)
        {
            return false;
        }

        Vector3 rayOrigin = headPOSITON.position;
        Vector3 targetPoint = gameManager.instance.player.transform.position + new Vector3(-0.5f, 1.5f, 0);
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

    /*
    bool CANsee_PLAYER()
    {
        Vector3 targetPos = gameManager.instance.player.transform.position + new Vector3(-0.5f, 1.5f, 0);

        playerDIRECTION = (targetPos - headPOSITON.position);

        ///Calling a new Vector3 to ignore the y in the player direction for the Y
        angleTO_PLAYER = Vector3.Angle(new Vector3(playerDIRECTION.x, 0, playerDIRECTION.z), transform.forward); //will return the angle to player

        //Will draw in the editor when the enemy see's a player
        Debug.DrawRay(headPOSITON.position, playerDIRECTION);

        RaycastHit hit;
        if (Physics.Raycast(headPOSITON.position, playerDIRECTION, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleTO_PLAYER <= FOV)
            {
                //This allows the enemy AI to follow the player to it's position
                //using the NavMeshSurface
                AGENT.SetDestination(gameManager.instance.playerScript.transform.position);

                //Must be activated within the enemy's stopping distance
                if (AGENT.remainingDistance <= AGENT.stoppingDistance)
                {
                    faceTARGET();
                    ShockwaveScript.TryShockwave();
                }

                shootTIMER += Time.deltaTime;

                if (angleTO_PLAYER <= shootFOV && shootTIMER >= shootRATE)
                {
                    enemySHOOT();
                }

              
                    
              

                AGENT.stoppingDistance = stoppingDistance_ORIGINAL;

                //returns true scenario for canSEE player
                return true;
            }
        }
        AGENT.stoppingDistance = 0;
        return false;
    }
    */
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player_IN_RANGE = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player_IN_RANGE = false;
            AGENT.stoppingDistance = 0;
        }
    }

    public void TakeDamage(int amount)
    {

        HP -= amount;
        StartCoroutine(flashDAMAGE_color());
        Audio.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);

        AGENT.SetDestination(gameManager.instance.player.transform.position);


        if (Time.time - lastDamageTime >= coverDamageCooldown)
        {
            lastDamageTime = Time.time;

            if (useCoverSystem && coverPoints.Count > 0)
            {
                isTakingCover = true;
                currentCoverPoint = GetRandomCoverPoint();

                if (currentCoverPoint != null)
                {
                    AGENT.SetDestination(currentCoverPoint.position);
                    currentCoverState = CoverState.MovingToCover;
                }
            }
            else
            {
                isTakingCover = false;
            }
        }

        if (HP <= 0)
        {
            //gameManager.instance.CountSpawner(); //Removing the gameGOALcount per enemy removed
            Destroy(gameObject); //takes whatever object this script is referencing and deletes from scene

            if (twin.HP == 0 || twin == null)
            {
                SceneManager.LoadScene(scenename);
            }
        }

    }

    /*
    public void WINcondition(MiniBoss2_Rager_AI twinRager)
    {
        twinRager = GetComponent<MiniBoss2_Rager_AI>();

        if (HP <= 0 || instance == null  && twinRager.HP <= 0 || twinRager == null)
        {
            SceneManager.LoadScene(scenename);
        }
    }
    */


    public void ApplyFreeze(float duration)
    {
        if (isFrozen) return;

        isFrozen = true;
        freezeTimer = duration;

        if (AGENT != null)
        {
            AGENT.isStopped = true;
        }
        if (anim != null)
        {
            anim.enabled = false;
        }
    }

    private void Unfreeze()
    {
        isFrozen = false;

        if (AGENT != null)
        {
            AGENT.isStopped = false;
        }
        if (anim != null)
        {
            anim.enabled = true;
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

        AGENT.isStopped = true;
        rb.isKinematic = false;
        rb.AddForce(force, ForceMode.Impulse);
    }

    /*
    public void Stun(float duration, Vector3 knockbackDIR)
    {
        StartCoroutine(StunCuroutine(duration, knockbackDIR));
    }

    IEnumerator StunCuroutine(float duration, Vector3 knockbackDIR)
    {
        AGENT.isStopped = true; //stop movement

        float timer = 0f;
        while (timer < duration)
        {
            transform.rotation = Quaternion.LookRotation(-knockbackDIR); //Face player
            transform.position += knockbackDIR * Time.deltaTime; //Move away
            timer += Time.deltaTime;
            yield return null;  
        }
        AGENT.isStopped = false; //resume movement
    }
    */

    IEnumerator flashDAMAGE_color()
    {
        MODEL.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        MODEL.material.color = colorOriginal;
    }

    void enemySHOOT()
    {
        shootTIMER = 0;

        anim.SetTrigger("Shoot");

    }

    /*
    public void sword_COLLIDE_ON()
    {
        SwordCOLLIDE.enabled = true;
    }
    public void sword_COLLIDE_OFF()
    {
        SwordCOLLIDE.enabled = false;
    }
    */

    public void createBULLET()
    {
        Audio.PlayOneShot(audShoot[Random.Range(0, audShoot.Length)], audShootVol);
        //Instantiate means to create something in the project in
        //real time in the scene
        Instantiate(BULLET, shootPOS.position, transform.rotation);
    }

    void faceTARGET()
    {
        Vector3 playerDir = (gameManager.instance.player.transform.position - transform.position).normalized;
        //This will fix the enemy jitters. Basically says face the player's x position and y position,
        //but do not use the player's y position
        Quaternion ROTATION = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, ROTATION, Time.deltaTime * faceTARGETspeed);

        //The problem with this code below is that it will cause an immediate snap
        //transform.rotation = Quaternion.LookRotation(playerDIRECTION);
    }

    IEnumerator playStep()
    {
        isPlayingStep = true;

        if (audStep.Length > 0)
        {
            Audio.PlayOneShot(audStep[Random.Range(0, audStep.Length)], audStepVol);
        }

        yield return new WaitForSeconds(0.3f);

        isPlayingStep = false;
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
            if (joltLine) joltLine.SetPosition(0, torsoPos.position);
            GameObject closestEnemy = null;
            Collider[] hitColliders = Physics.OverlapSphere(headPOSITON.position, 5);
            float shortestDistance = Mathf.Infinity;
            EnemyAI enemyCheck = null;
            foreach (var hit in hitColliders)
            {
                if (hit.CompareTag("Enemy"))
                {
                    enemyCheck = hit.GetComponent<EnemyAI>();
                    float distance = Vector3.Distance(headPOSITON.position, hit.transform.position);
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
            if (joltLine) joltLine.SetPosition(1, closestEnemy.transform.position);
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