using System.Collections;
using UnityEngine;

public class damage : MonoBehaviour
{
    enum damageType { moving, stationary, DOT, homing, AOE, melee }
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    [SerializeField] float explosionRad;
    [SerializeField] float fuseTime;
    [SerializeField] GameObject explosionEffect;

    [SerializeField] int damageAmount;
    [SerializeField] float damageRate;
    [SerializeField] int speed;
    [SerializeField] float destroyTime;

    [SerializeField] float meleeTimer;

    bool isDamaging;
    void Start()
    {
        if (type == damageType.moving || type == damageType.homing || type == damageType.AOE)
        {
            Destroy(gameObject, destroyTime);
        }
        if (type == damageType.moving || type == damageType.AOE)
        {
            rb.linearVelocity = transform.forward * speed;
        }

        if (type == damageType.AOE)
        {
            Destroy(gameObject, destroyTime);
        }
    }





    // Update is called once per frame
    void Update()
    {
        if (type == damageType.homing)
        {
            rb.linearVelocity = (gameManager.instance.player.transform.position - transform.position).normalized * speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if(type == damageType.AOE)
        {
            Explode();
        }
        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null && (type == damageType.stationary || type == damageType.homing || type == damageType.moving || type == damageType.melee))
        {
            dmg.TakeDamage(damageAmount);
        }

        if (type == damageType.moving || type == damageType.homing)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null && type == damageType.DOT)
        {
            if (!isDamaging)
            {
                StartCoroutine(damageOther(dmg));
            }
        }
    }

    void Explode()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRad);
        foreach(var hit in hitColliders)
        {
            IDamage dmg = hit.GetComponent<IDamage>();
            if(dmg != null)
            {
                dmg.TakeDamage(damageAmount);
            }
        }

        Destroy(gameObject);
    }

    IEnumerator damageOther(IDamage d)
    {
        isDamaging = true;
        d.TakeDamage(damageAmount);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }
}


