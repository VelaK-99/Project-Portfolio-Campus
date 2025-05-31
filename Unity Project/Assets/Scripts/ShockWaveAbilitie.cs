using Unity.VisualScripting;
using UnityEngine;

public class ShockWaveAbilitie : MonoBehaviour
{
    [SerializeField] GameObject SHOCKWAVEparticles;
    [SerializeField] Animator shockWaveAnim;
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

    // Update is called once per frame
    void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
        
        if (Input.GetKeyDown(KeyCode.T)&&cooldownTimer <= 0f)
        {
            gameManager.instance.playerScript.handsAnimator.SetTrigger("Cast");
            shockWaveAnim.SetTrigger("Cast");
            Shock();
            cooldownTimer = cooldown;
            StartCoroutine(gameManager.instance.UpdateShockwaveIcon(cooldown));
            aud.PlayOneShot(audShock[Random.Range(0, audShock.Length)], audShockVol);
        }
    }

    /// <summary>
    /// Helps to visualize if radius is wrong
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    void Shock()
    {
        Instantiate(SHOCKWAVEparticles, transform.position, Quaternion.identity);

        Collider[] enamies = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider e in enamies)
        {
            EnemyAI enemy = e.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                Vector3 dir = (enemy.transform.position - transform.position).normalized;
                enemy.TakeDamage(damage);
                enemy.Stun(duration, dir * knockback);
            }

            MiniBoss2_Stomper_AI stomper = e.GetComponent<MiniBoss2_Stomper_AI>();
            if (stomper != null)
            {
                Vector3 dir = (stomper.transform.position - transform.position).normalized;
                stomper.TakeDamage(damage);
                stomper.Stun(duration, dir * knockback);
            }

            MiniBoss2_Rager_AI rager = e.GetComponent<MiniBoss2_Rager_AI>();
            if (rager != null)
            {
                Vector3 dir = (rager.transform.position - transform.position).normalized;
                rager.TakeDamage(damage);
                rager.Stun(duration, dir * knockback);
            }
        }
    }
}
