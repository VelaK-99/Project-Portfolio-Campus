using UnityEngine;
using UnityEngine.AI;

public class MiniBoss2_Shockwave : MonoBehaviour
{
    
    [SerializeField] Animator StomperANIM;
    //[SerializeField] NavMeshAgent StomperAGENT;
    [SerializeField] private float animTRANspeed = 5f;
    

    [SerializeField] GameObject SHOCKWAVEparticles;
    [SerializeField] float radius;
    [SerializeField] float knockback;
    [SerializeField] int damage;
    [SerializeField] int cooldown;
    [SerializeField] float duration;
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audShock;
    [Range(0f, 2f)][SerializeField] float audShockVol;
    float cooldownTimer = 0f;  

    GameObject player;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }


    // Update is called once per frame
    public bool TryShockwave()
    {
            if (cooldownTimer <= 0f && player != null && Vector3.Distance(transform.position, player.transform.position) <= radius)
            {
                StomperANIM.SetTrigger("Shockwave");
                cooldownTimer = cooldown;
                return true;
            }

            return false;
    }

    /// <summary>
    /// Helps to visualize if radius is wrong
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    void Shock()
    {
        Instantiate(SHOCKWAVEparticles, transform.position, Quaternion.identity);
        aud.PlayOneShot(audShock[Random.Range(0, audShock.Length)], audShockVol);

        Collider[] players = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider user in players)
        {
            PlayerScript player = user.GetComponent<PlayerScript>();
            if (player != null)
            {
                Vector3 dir = (player.transform.position - transform.position).normalized;
                player.TakeDamage(damage);
                player.Stun(duration, dir * knockback);
            }
        }
    }

    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerScript player = other.GetComponent<PlayerScript>();

            if (player != null)
            {
                player.TakeDamage(damage);
                player.Stun(duration, other.transform.position - transform.position);
                Shock();
            }
        }
    }
    */

}
