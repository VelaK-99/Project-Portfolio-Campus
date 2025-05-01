using UnityEngine;
using System.Collections;




public class Destructible : MonoBehaviour, IDamage
{
    [SerializeField] Renderer MODEL;

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
        MODEL.material.color = Color.cyan;
        yield return new WaitForSeconds(0.1f);
        MODEL.material.color = colorORIGINAL;
    }
}
