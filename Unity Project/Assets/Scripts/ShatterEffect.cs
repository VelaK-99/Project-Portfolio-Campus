using UnityEngine;
using System.Collections;




public class ShatterEffect : MonoBehaviour, IDamage
{
    [Header("Setup")]
    [SerializeField] Renderer MODEL;
    [SerializeField] private GameObject fracturedPrefab;
    [SerializeField] private AudioClip fractureSound;
    [SerializeField] private AudioSource SoundBase;
    [SerializeField] private ParticleSystem fractureEffect;

    [Header("Stats")]
    [SerializeField] private int maxHP = 20;

    private int currentHP;
    private Color ORIGINALcolor;

    void Start()
    {
        currentHP = maxHP;
        ORIGINALcolor = MODEL.material.color;
    }


    public void TakeDamage(int amount)
    {
        currentHP -= amount;

        float HPratio = Mathf.Clamp01((float)currentHP / maxHP);

        StartCoroutine(flashDAMAGE_color());

        if (currentHP <= 0)
        {
            BreakObject();
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
           { MODEL.material.color = ORIGINALcolor; }
    }

    void UpdateOpacity(float alpha)
    {
        if (MODEL == null || MODEL.material == null) return;

        Color NEWcolor = MODEL.material.color;
        NEWcolor.a = alpha;
        MODEL.material.color = NEWcolor;
    }

    void BreakObject()
    {
        float SoundLength = 0f;

        if (fractureSound)
        {
            AudioSource.PlayClipAtPoint(fractureSound, transform.position, 15f);
        }

        if (fractureEffect)
        {
            Instantiate(fractureEffect, transform.position, Quaternion.identity).Play();
        }

        MODEL.enabled = false;

            
        if (fracturedPrefab)
        {

            fracturedPrefab.transform.SetParent(null); //detach from parent
            fracturedPrefab.transform.position = transform.position;
            fracturedPrefab.transform.rotation = transform.rotation;
            fracturedPrefab.SetActive(true);

            foreach (Collider collide in GetComponentsInChildren<Collider>())
                {
                    collide.enabled = false;
                }

            Destroy(fracturedPrefab, 3f);
            //StartCoroutine(FADEandDESTROY(fracturedPrefab));
        }

        float destroyDelay = Mathf.Max(SoundLength + 0.1f, 0.5f);

        Destroy(gameObject);

    }

    /*
    IEnumerator FADEandDESTROY(GameObject fractured)
    {
        float delayBEFOREfade = 1.5f; //time to let shards settle
        float duration = 1f;
        float elapsed = 0f;

        MeshRenderer[] renderers = fractured.GetComponentsInChildren<MeshRenderer>();

        Material[] mats = new Material[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            mats[i] = renderers[i].material = new Material(renderers[i].material);
        }

        yield return new WaitForSeconds(delayBEFOREfade);

        foreach (Collider col in fractured.GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }

        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);

            foreach (var mat in mats)
            {
                if (mat.HasProperty("_Color"))
                {
                    Color col = mat.color;
                    col.a = alpha;
                    mat.color = col;
                }
            }

            elapsed += Time.deltaTime;
            yield return null;

        }


        Destroy(fractured);

    }
    */
}