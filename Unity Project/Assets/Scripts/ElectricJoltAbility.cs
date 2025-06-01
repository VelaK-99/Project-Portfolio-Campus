using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class ElectricJolt : MonoBehaviour
{
    public bool isUnlocked;
    [SerializeField] int joltChainLength;
    [SerializeField] int damageAmount;
    [SerializeField] public int coolDownTimer;
    [SerializeField] int electricCastDistance;
    float joltDuration = 0.5f;
    public LineRenderer joltLine;
    public Transform joltOrigin;
    [SerializeField] ParticleSystem hitEffect;
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audJolt;
    [Range(0, 100)][SerializeField] float audJoltVol;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("AbilityJolt") && isUnlocked)
        {
            gameManager.instance.playerScript.handsAnimator.SetTrigger("Cast");
            StartCoroutine(gameManager.instance.UpdateElectricIcon(coolDownTimer));
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
            Instantiate(hitEffect, hit.point, Quaternion.identity);
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
        else
        {
            joltLine.SetPosition(1, gameManager.instance.playerScript.cam.position + (gameManager.instance.playerScript.cam.forward * electricCastDistance));
            StartCoroutine(ShootJoltRay());
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