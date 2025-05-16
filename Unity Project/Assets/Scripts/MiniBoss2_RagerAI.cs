using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MiniBoss2_Rager_AI : MonoBehaviour, IDamage
{
    [Header("===== Model Info =====")]
    /// <summary>
    /// Used to tie in a model/mesh for the enemy AI script
    /// </summary>
    [SerializeField] Renderer MODEL;

    /// <summary>
    /// This is tied to NavMeshSurface to allow Unity's pathfinding
    /// </summary>
    [SerializeField] NavMeshAgent AGENT;

    /// <summary>
    /// Where the new raycast will come from
    /// </summary>
    [SerializeField] Transform headPOSITON;


    /// <summary>
    /// The controller that is attached to the mesh
    /// </summary>
    [SerializeField] Animator anim;

    [SerializeField] float animTRANspeed;

    [SerializeField] float HP;

    [Header("===== Enemy Stats =====")]
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


    [Header("===== Audio =====")]
    [SerializeField] AudioSource Audio;
    [SerializeField] AudioClip[] audShoot;
    [Range(0f, 2f)][SerializeField] float audShootVol;
    [SerializeField] AudioClip[] audHurt;
    [Range(0f, 2f)][SerializeField] float audHurtVol;
    [SerializeField] AudioClip[] audStep;
    [Range(0f, 2f)][SerializeField] float audStepVol;

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






    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        colorORIGINAL = MODEL.material.color;

        //Left out to allow for the spawner to update the game goal
        //Game_Management.INSTANCE.updateGAMEgoal(1); //adding 1 to the gameGOALamount
        //referencing amount of enemies


        startingPOSITION = transform.position;
        stoppingDistance_ORIGINAL = AGENT.stoppingDistance;


    }

    // Update is called once per frame
    void Update()
    {

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
        ranPOS += startingPOSITION;

        ///To know exactly where the mesh is at, to go only on the map where a NavMesh is at
        NavMeshHit hit;

        ///Selecting a position in the Navmesh; the 1 is for the default layer, not using multiple navmesh layers
        NavMesh.SamplePosition(ranPOS, out hit, roam_DISTANCE, 1);

        //The location in which the NPC should go
        AGENT.SetDestination(hit.position);
    }



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

        if (HP <= 0)
        {
            //gameManager.instance.CountSpawner(); //Removing the gameGOALcount per enemy removed
            Destroy(gameObject); //takes whatever object this script is referencing and deletes from scene
        }
    }

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

    IEnumerator flashDAMAGE_color()
    {
        MODEL.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        MODEL.material.color = colorORIGINAL;
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
        //This will fix the enemy jitters. Basically says face the player's x position and y position,
        //but do not use the player's y position
        Quaternion ROTATION = Quaternion.LookRotation(new Vector3(playerDIRECTION.x, transform.position.y, playerDIRECTION.z));
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

}