using UnityEngine;
using System.Collections;




public class Destructible : MonoBehaviour, IDamage
{
    [Header("Setup")]
    [SerializeField] Renderer MODEL;
    [SerializeField] private GameObject fracturedPrefab;
    [SerializeField] private AudioClip fractureSound;

    Color colorORIGINAL;

    [SerializeField] int HP;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorORIGINAL = MODEL.material.color;
    }


    public void TakeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashDAMAGE_color());

        if (HP <= 0)
        {
            Destroy(gameObject); //takes whatever object this script is referencing and deletes from scene
        }
    }

    IEnumerator flashDAMAGE_color()
    {
        if (MODEL == null || MODEL.material == null)
        {
            yield break;
        }

        MODEL.material.color = Color.cyan;
        yield return new WaitForSeconds(0.1f);

        if (MODEL != null && MODEL.material != null)
        MODEL.material.color = colorORIGINAL;
    }
}
