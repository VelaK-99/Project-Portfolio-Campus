using UnityEngine;

public class ShockWaveAbilitie : MonoBehaviour
{

    [SerializeField] float radius;
    [SerializeField] float knockback;
    [SerializeField] int damage;
    [SerializeField] int cooldown;
    [SerializeField] float duration;
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audShock;
    [Range(0, 100)][SerializeField] float audShockVol;


    float cooldownTimer = 0f;

    bool canUse;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
            if (Input.GetKeyDown(KeyCode.T)&&cooldownTimer <= 0f)
        
        {
            Shock();
            cooldownTimer = cooldown;
            Debug.Log("using");
            aud.PlayOneShot(audShock[Random.Range(0, audShock.Length)], audShockVol);
        }
      
       
    }

    void Shock()
    {
        Collider[] enamies = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider e in enamies)
        {
            EnemyAI enemy = e.GetComponent<EnemyAI>();
           

            if (enemy != null)
            {
                Vector3 dir = (enemy.transform.position - transform.position).normalized;
                enemy.TakeDamage(damage);
                enemy.Stun(duration,dir*knockback);
               
               

            }
        }
    
}


}
