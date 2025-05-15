using UnityEngine;


public class DashAbility : MonoBehaviour
{
    [SerializeField] float dashSpeed;
    [SerializeField] float dashTime ;
    [SerializeField] float dashCooldown;
    [SerializeField] int maxDashes;

    CharacterController controller;
    Vector3 dashDirection;
    bool isDashing = false;
    float dashTimer;

    int availableDashes;
   float[] cooldownTimers;

    float doubleTapTime=0.3f;
    KeyCode lastKey;
    float lastTapTime;


    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audDash;
    [Range(0, 100)][SerializeField] float audDashVol;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        availableDashes = maxDashes;
        cooldownTimers = new float[maxDashes];
    }

    void Update()
    {
        if (!isDashing)
            HandleDoubleTap();

        HandleCooldowns();

        if (isDashing)
        {
            controller.Move(dashDirection * dashSpeed * Time.deltaTime);
            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0f)
            {
                isDashing = false;
            }
                
        }
    }

    void HandleDoubleTap()
    {
        KeyCode[] keys = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };

        foreach (var key in keys)
        {
            if (Input.GetKeyDown(key))
            {
                if (key == lastKey && Time.time - lastTapTime <= doubleTapTime)
                {
                    TryDash(GetDirectionFromKey(key));
                }

                lastKey = key;
                lastTapTime = Time.time;
            }
        }
    }

    Vector3 GetDirectionFromKey(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.W:return transform.forward;
            case KeyCode.S: return -transform.forward;
            case KeyCode.A:return -transform.right;
            case KeyCode.D: return transform.right;
            default:return Vector3.zero;
        }
    }

    void TryDash(Vector3 direction)
    {
        if (availableDashes > 0)
        {
            isDashing = true;
            dashTimer = dashTime;
            dashDirection = direction.normalized;

            int usedIndex = maxDashes - availableDashes;
            cooldownTimers[usedIndex] = dashCooldown;

            availableDashes--;
            aud.PlayOneShot(audDash[Random.Range(0, audDash.Length)], audDashVol);
        }
    }

    void HandleCooldowns()
    {
        for (int i = 0; i < cooldownTimers.Length; i++)
        {
            if (cooldownTimers[i] > 0f)
            {
                cooldownTimers[i] -= Time.deltaTime;

                if (cooldownTimers[i] <= 0f)
                {
                    availableDashes++;
                    cooldownTimers[i] = 0f;
                }
            }
        }
    }
}
