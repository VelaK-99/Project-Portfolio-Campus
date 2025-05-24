using UnityEngine;

public class MeleeANIM : MonoBehaviour
{
    [SerializeField] Animator animator;
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
        if (Input.GetKeyDown(KeyCode.Q) && meleeTimer <= 0f)
        {
            animator.SetTrigger("Melee1");
            meleeTimer = meleeCooldown;
        }
    }

}
