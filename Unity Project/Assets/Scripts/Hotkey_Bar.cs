using System.Collections.Generic;
using UnityEngine;


public class Hotkey_Bar : MonoBehaviour
{

    public Hotkey_slots_UI[] slots_ui = new Hotkey_slots_UI[3];

    public int activeSLOT = -1;

    public PlayerScript playerSCRIPT;

    public void AssignAvailableSLOT(gunStats pickedWEAPON)
    {
        for (int i = 0; i < gameManager.instance.playerScript.arsenal.Count; i++)
        {
            if (gameManager.instance.playerScript.arsenal[i] == null)
            {
                gameManager.instance.playerScript.arsenal[i] = pickedWEAPON;

                gameManager.instance.InventorySLOTS[i].SetActive(true);

                slots_ui[i].SetSLOT(pickedWEAPON);
            }
        }
    }

    /*
    private void Start()
    {
        foreach (var slot in slots_ui)
        {
           slot.SetSLOT(null); //clear all UI on start
        }
    }
    */

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("num1")) EQUIPslot(0);
        if (Input.GetButtonDown("num2")) EQUIPslot(1);
        if (Input.GetButtonDown("num3")) EQUIPslot(2);

        
         
        //-------To display Ammo Constantly outside of reload/shoot
        for (int i = 0; i < gameManager.instance.playerScript.arsenal.Count; i++)
        {
            /*
            gunStats weapon = gameManager.instance.playerScript.arsenal[i];
            if (weapon != null)
            {
                slots_ui[i].UpdateAmmo(weapon);
            }
            */

            
            if (gameManager.instance.playerScript.arsenal[i] == null)
            {
                slots_ui[i].SetSLOT(gameManager.instance.playerScript.arsenal[i]);
            }
            
        }
        
    }

    public void EQUIPslot(int index)
    {
        if (gameManager.instance.playerScript.arsenal[index] != null)
        {
            {
                activeSLOT = index;
                playerSCRIPT.ChangeGun(index);
                refreshUI();
            }
        }
    }

    public void refreshUI()
    {
        if (activeSLOT >= 0 && gameManager.instance.playerScript.arsenal[activeSLOT] != null)
        {
            slots_ui[activeSLOT].UpdateAmmo(gameManager.instance.playerScript.arsenal[activeSLOT]);
        }
    }
}