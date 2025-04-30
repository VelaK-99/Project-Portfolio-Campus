using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class gameManager : MonoBehaviour
{

    public static gameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] public TMP_Text TotalAmmo;
    [SerializeField] public TMP_Text CurrentAmmo;

    public GameObject emptyGunText;
    public GameObject playerSpawnPos;
    public GameObject checkpointPopup;
    public GameObject enemyCountTextObject;
    public GameObject reloadGunText;
    public GameObject reloadingGunText;
    [SerializeField] TMP_Text RoomCount;
    public int currentRoom = 1;

    [SerializeField] GameObject hotkeyBAR;

    public GameObject[] InventorySLOTS;
    public int slotINDEX = -1;

    public GameObject interactUI;
    [SerializeField] TMP_Text interactionText; // Changeable text that you can use InteractTextUpdate()

    public GameObject player;
    public PlayerScript playerScript;
    public Image playerHPBar;
    public GameObject playerDamageScreen;

    public int totalSpawners;
    private int clearedSpawners;

    public List<GameObject> totalEnemies = new List<GameObject> ();
    

    public bool isPaused;

    float timeScaleOrig;    

    float gameGoalCount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerScript>();
        timeScaleOrig = Time.timeScale;
        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("num1"))
        {
            ToggleSlot(0);
        }       
        
        if (Input.GetButtonDown("num2"))
        {
           ToggleSlot(1);
        }

        if (Input.GetButtonDown("num3"))
        {
            ToggleSlot(2);
        }


        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                stateUnpause();
                menuActive.SetActive(false);
                menuActive = null;
            }
        }

        if (Input.GetButtonDown("TAB"))
        {
            if (hotkeyBAR.activeSelf == true)
            {
                hotkeyBAR.SetActive(false);
            }
            else
            {
                hotkeyBAR.SetActive(true);
            }
        }
        
        //Preventing deselection from mouse clicks by reapplying selection
        if (slotINDEX >= 0)
        {
            if (EventSystem.current.currentSelectedGameObject != InventorySLOTS[slotINDEX])
            {
                EventSystem.current.SetSelectedGameObject(InventorySLOTS[slotINDEX]);
            }
        }
        
    }

    
    /// <summary>
    /// Function for deselecting(unequipping) selected object
    /// </summary>
    /// <param name="index"></param>
    public void ToggleSlot(int index)
    {
        if (slotINDEX == index)
        {
            //Deselect if some key is pressed again
            EventSystem.current.SetSelectedGameObject(null);
            slotINDEX = -1;
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(InventorySLOTS[index]);
            hotkeyBAR.GetComponent<Hotkey_Bar>().EQUIPslot(index);
            slotINDEX = index;

            playerScript.ChangeGun(index);
        }
    }
    

    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    } //State of the game when paused

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    } // Returns game to default state when unpaused

     
    public void UpdateRoom()
    {
        currentRoom++;
        RoomCount.text = "Room: " + currentRoom;
    }

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    } //Brings up the lose menu

    public void youWin()
    {
        statePause();
        menuActive = menuWin;
        menuActive.SetActive(true);
    }

    public void InteractTextUpdate(string text) // Use to update the interaction text
    {
        interactionText.text = text;
    }

    public void CountSpawner()
    {
        totalSpawners++;
    }

    public void ClearSpawners()
    {
        clearedSpawners++;
        CheckIfWin();
    }

    public void AddEnemy(GameObject enemy)
    {
        totalEnemies.Add(enemy);
    }

    public void RemoveEnemy(GameObject enemy)
    {
        totalEnemies.Remove(enemy);
        totalEnemies.RemoveAll(item => item == null);
        CheckIfWin();
    }

    public void CheckIfWin()
    {
        if(clearedSpawners >= totalSpawners && totalEnemies.Count <= 0)
        {
            youWin();
        }
    }
}
