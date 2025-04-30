using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hotkey_slots_UI : MonoBehaviour
{
    public Image gun_image;
    public TMP_Text current_ammoTEXT;
    public TMP_Text total_ammoTEXT;

    public void SetSLOT(gunStats gun)
    {
        if (gun != null)
        {
            gun_image.sprite = gun.Image.GetComponent<Image>().sprite;
            gun_image.enabled = true;
            current_ammoTEXT.text = gun.currentAmmo.ToString("F0");
            total_ammoTEXT.text = gun.totalAmmo.ToString("F0");
        }
        else
        {
            gun_image.enabled = false;
            current_ammoTEXT.text = "";
            total_ammoTEXT.text = "";
        }
    }
}
