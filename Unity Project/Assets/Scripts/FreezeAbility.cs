using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class FreezeAbility : MonoBehaviour
{
    [SerializeField] int freezeDuration = 3;
    [SerializeField] int bossFreezeDuration = 2;
    [SerializeField] float burstDuration = 1.5f;
    [SerializeField] float freezeRange = 20f;
    [SerializeField] float freezeAngle = 45f;
    [SerializeField] int cooldownTimer = 5;
    [SerializeField] public bool unlocked;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] GameObject iceAbilityEffect;
    [SerializeField] Transform shootOrigin; // Set this to the player camera or gun position

    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audFroozen;
    [Range(0, 100)][SerializeField] float audFroozenVol;

    private bool isBurstActive = false;
    private GameObject activeIceEffect;

    // Make this method PUBLIC so you can call it from PlayerController
    public void ActivateFreeze()
    {
        if (!isBurstActive)
        {
            StartCoroutine(ActivateFreezeBurst());
            StartCoroutine(gameManager.instance.UpdateFreezeIcon(cooldownTimer));
            StartCoroutine(AbilityCooldown());
        }
    }

    private IEnumerator ActivateFreezeBurst()
    {
        isBurstActive = true;
        if (aud != null)
            aud.PlayOneShot(audFroozen[Random.Range(0, audFroozen.Length)], audFroozenVol);


        if (iceAbilityEffect != null)
        {
            activeIceEffect = Instantiate(iceAbilityEffect, new Vector3(shootOrigin.position.x,shootOrigin.position.y - 0.5f, shootOrigin.position.z +0.2f), shootOrigin.rotation);
            activeIceEffect.transform.parent = shootOrigin; // Make it follow the player
            Destroy(activeIceEffect, burstDuration);
        }

        float timer = 0f;
        while (timer < burstDuration)
        {
            timer += Time.deltaTime;
            FreezeEnemiesInCone();
            yield return null;
        }
        isBurstActive = false;
    }

    // Method to freeze enemies in a cone in front of the player
    private void FreezeEnemiesInCone()
    {
        Collider[] enemies = Physics.OverlapSphere(shootOrigin.position, freezeRange, enemyLayer);

        foreach (Collider enemy in enemies)
        {
            Vector3 directionToEnemy = (enemy.transform.position - shootOrigin.position).normalized;
            float angleToEnemy = Vector3.Angle(shootOrigin.forward, directionToEnemy);

            if (angleToEnemy <= freezeAngle / 2)
            {
                if (enemy.CompareTag("Mecha Hitler"))
                {
                    mechaHitlerAI mechaHitler = enemy.GetComponent<mechaHitlerAI>();
                    if (mechaHitler != null)
                    {
                        mechaHitler.ApplyFreeze(bossFreezeDuration);
                        mechaHitler.FrozenVisual(bossFreezeDuration);
                    }
                }
                else if(enemy.CompareTag("Enemy"))
                {
                    EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
                    if (enemyAI != null)
                    {
                        enemyAI.ApplyFreeze(freezeDuration);
                        enemyAI.FrozenVisual(freezeDuration);
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (shootOrigin == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(shootOrigin.position, freezeRange);

        Vector3 leftLimit = Quaternion.Euler(0, -freezeAngle / 2, 0) * shootOrigin.forward * freezeRange;
        Vector3 rightLimit = Quaternion.Euler(0, freezeAngle / 2, 0) * shootOrigin.forward * freezeRange;

        Gizmos.DrawLine(shootOrigin.position, shootOrigin.position + leftLimit);
        Gizmos.DrawLine(shootOrigin.position, shootOrigin.position + rightLimit);
    }

    IEnumerator AbilityCooldown()
    {
        unlocked = false;
        yield return new WaitForSeconds(cooldownTimer);
        unlocked = true;
    }
}
