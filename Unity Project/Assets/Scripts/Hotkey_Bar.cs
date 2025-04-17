using UnityEngine;


public class Hotkey_Bar : MonoBehaviour
{
    public Weapons[] weaponSLOTS = new Weapons[3];
    public int activeSLOT = -1;


    public void AssignAvailableSLOT(Weapons pickedWEAPON)
    {
        for (int i = 0; i < weaponSLOTS.Length; i++)
        {
            if (weaponSLOTS[i] == null)
            {
                weaponSLOTS[i] = pickedWEAPON;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("num1")) EQUIPslot(0);
        if (Input.GetButtonDown("num2")) EQUIPslot(1);
        if (Input.GetButtonDown("num3")) EQUIPslot(2);
    }


    void EQUIPslot(int index)
    {
        if (weaponSLOTS[index] != null)
        {
            {
                activeSLOT = index;
                //FindObjectOfType<PlayerScript>().UpdateWeapon(weaponSLOTS[index]);
            }
        }
    }
}