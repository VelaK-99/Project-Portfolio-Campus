using UnityEngine;
using UnityEngine.UI;

public class RageUI : MonoBehaviour
{
    [SerializeField] Image rageFill;

    public void UpdateRageBar(float current, float max)
    {
        rageFill.fillAmount = current / max;
    }
}
