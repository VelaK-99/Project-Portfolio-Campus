using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Hotkey_slots_UI : MonoBehaviour
{
    public static Hotkey_slots_UI instance;

    public Image gun_image;
    public TMP_Text current_ammoTEXT;
    public TMP_Text total_ammoTEXT;

    public void Awake()
    {
        instance = this;
    }

    public void SetSLOT(gunStats gun)
    {
        if (gun != null)
        {
            gun_image.sprite = gun.Image;
            gun_image.enabled = true;
            current_ammoTEXT.text = gun.currentAmmo.ToString("F0");
            total_ammoTEXT.text = gun.totalAmmo.ToString("F0");
        }
        else
        {
            gun_image.sprite = null; //Clear sprite
            gun_image.enabled = false; //Hide image
            current_ammoTEXT.text = "";
            total_ammoTEXT.text = "";
        }
    }

    public void UpdateAmmo(gunStats gun)
    {
        current_ammoTEXT.text = gun.currentAmmo.ToString("F0");
        total_ammoTEXT.text = gun.totalAmmo.ToString("F0");
        gun_image.sprite = gun.Image;
    }
}
