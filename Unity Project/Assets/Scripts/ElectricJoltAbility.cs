using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class ElectricJolt : MonoBehaviour
{
    public bool isUnlocked;
    [SerializeField] int joltChainLength;
    [SerializeField] int damageAmount;
    [SerializeField] int coolDownTimer;
    [SerializeField] int electricCastDistance;
    float joltDuration = 0.5f;
    public LineRenderer joltLine;
    public Transform joltOrigin;

    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audJolt;
    [Range(0, 100)][SerializeField] float audJoltVol;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("AbilityJolt") && isUnlocked)
        {
            CastElectricJolt();
        }
    }

    void CastElectricJolt()
    {
        joltLine.SetPosition(0, joltOrigin.position);
        aud.PlayOneShot(audJolt[Random.Range(0, audJolt.Length)], audJoltVol);
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, electricCastDistance))
        {
            joltLine.SetPosition(1, hit.point);
            StartCoroutine(ShootJoltRay());
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            IElectricJolt jolt = hit.collider.GetComponent<IElectricJolt>();
            
            if (dmg != null) dmg.TakeDamage(damageAmount);
            if (jolt != null)
            {
                EnemyAI enemyScript = hit.collider.GetComponent<EnemyAI>();
                if (enemyScript != null)
                {
                    enemyScript.hasBeenJolted = true;
                }
                jolt.JoltEffect(damageAmount,joltChainLength);
            }
        }
        StartCoroutine(CoolDown());
    }

    IEnumerator ShootJoltRay()
    {
        joltLine.enabled = true;
        yield return new WaitForSeconds(joltDuration);
        joltLine.enabled = false;
    }

    IEnumerator CoolDown()
    {
        isUnlocked = false;
        yield return new WaitForSeconds(coolDownTimer);
        isUnlocked = true;
    }
}