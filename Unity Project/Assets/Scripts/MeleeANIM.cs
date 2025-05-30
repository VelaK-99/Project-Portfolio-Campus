using UnityEngine;
using System.Collections;

public class MeleeANIM : MonoBehaviour
{
    [SerializeField] Animator animator;

    //The point of these two GameObjects is to have the gun follow with melee, without the gun will appear static with melee
    public GameObject WEAPONmodel; //reference to main gun
    //public GameObject WEAPONprefab; //prefab of the same gun
    public Transform meleeHANDtransform; //Where the weapon appears
    //private GameObject meleeWeapon = null;

    [SerializeField] AudioSource MeleeSound;
    [SerializeField] AudioClip[] audStrikes;
    [Range(0, 2f)] public float audStrikeVol;

    private float meleeTimer = 0f;
    public float meleeCooldown = 3f;

    // Update is called once per frame
    void Update()
    {
        meleeTimer -= Time.deltaTime;
        Melee();
    }

    public void Melee()
    {
        if (Input.GetButtonDown("Melee") && meleeTimer <= 0f)
        {
            meleeTimer = meleeCooldown;

            SetGunVisible(false);

            //meleeWeapon = Instantiate(WEAPONprefab, meleeHANDtransform.position, meleeHANDtransform.rotation, meleeHANDtransform);
            
            animator.SetTrigger("Melee1");
            MeleeSound.PlayOneShot(audStrikes[Random.Range(0, audStrikes.Length)], audStrikeVol);

            StartCoroutine(EndMeleeRoutine());
        }
    }

    private IEnumerator EndMeleeRoutine()
    {
        float animDuration = animator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(animDuration);

        /*
        if (meleeWeapon != null )
        {
            Destroy(meleeWeapon);
        }
        */

        SetGunVisible(true);

    }

    /// <summary>
    /// Method to just hide the gun, not the children (arms and hands)
    /// </summary>
    /// <param name="isVisible"></param>
    private void SetGunVisible(bool isVisible)
    {
        if (WEAPONmodel != null)
        {
            MeshRenderer[] renderers = WEAPONmodel?.GetComponentsInChildren<MeshRenderer>(true);
            foreach (var renderer in renderers)
            {
                renderer.enabled = isVisible;
            }
        }
    }
}
