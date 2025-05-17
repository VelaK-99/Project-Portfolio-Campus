using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] Image BossHealthBarFill;
    private IBoss boss;

    public void AssignBoss(IBoss currentBoss)
    {
        boss = currentBoss;
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (boss != null)
        {
            float fillAmt = (float)boss.CurrentHealth / boss.MaxHealth;
            BossHealthBarFill.fillAmount = fillAmt;
        }
    }
}
