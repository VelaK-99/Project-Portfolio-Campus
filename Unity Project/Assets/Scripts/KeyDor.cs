using UnityEngine;

public class KeyDor : MonoBehaviour
{
    [SerializeField]GameObject key;
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audKey;
    [Range(0, 100)][SerializeField] float audkeyVol;
    bool IsPlayerNear=false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)&&IsPlayerNear)
        {
            gameManager.instance.haskey = true;
            aud.PlayOneShot(audKey[Random.Range(0, audKey.Length)], audkeyVol);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IsPlayerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IsPlayerNear = false;
        }
    }
}
