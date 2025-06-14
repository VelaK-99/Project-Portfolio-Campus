using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Collections;

public class gameManager : MonoBehaviour
{

    public static gameManager instance;

    [Header ("===== Menu =====")]
    [SerializeField] GameObject menuActive;
    [SerializeField] public GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] public GameObject hudPanel;
    public GameObject painelSettings;
    [SerializeField] private GameObject firstButtonMenu;


    [Header("===== Player Stats =====")]
    [SerializeField] public TMP_Text TotalAmmo;
    [SerializeField] public TMP_Text CurrentAmmo;
    public GameObject player;
    public PlayerScript playerScript;
    public Image playerHPBar;
    public GameObject playerDamageScreen;
    public GameObject playerRageScreen;

    [Header("===== Weapon Satats =====")]
    public GameObject emptyGunText;
    public GameObject playerSpawnPos;
    public GameObject checkpointPopup;
    public GameObject reloadGunText;
    public GameObject reloadingGunText;

    [Header("===== Enemy Stats =====")]
    [SerializeField] TMP_Text RoomCount;
    public int currentRoom = 1;
    public int totalSpawners;
    private int clearedSpawners;
    public List<GameObject> totalEnemies = new List<GameObject>();

    [Header("===== Inventory =====")]
    [SerializeField] GameObject hotkeyBAR;
    public GameObject[] InventorySLOTS;
    public int slotINDEX = -1;

    [Header("===== Interacts =====")]
    public GameObject interactUI;
    [SerializeField] TMP_Text interactionText; // Changeable text that you can use InteractTextUpdate()

    [Header("===== MiniBoss/Boss Fight =====")]
    public GameObject bossHealthBar;

    [Header("===== Save System =====")]
    public List<gunStats> allGuns;
    [Header("===== Objectives =====")]
    public TMP_Text objectiveText;

    [Header("===== Abilities =====")]
    [SerializeField] Image abilityOne;
    Color abilityOneColorOrig;
    [SerializeField] Image abilityTwo;
    Color abilityTwoColorOrig;
    [SerializeField] Image abilityThree;
    Color abilityThreeColorOrig;

    [Header("===== Other =====")]
    public bool isPaused;
    float timeScaleOrig;    
    float gameGoalCount;
    public bool haskey = false;

    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;
        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerScript>();
        abilityOneColorOrig = abilityOne.color;
        abilityTwoColorOrig = abilityTwo.color;
        abilityThreeColorOrig = abilityThree.color;
    }


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
            if (menuActive == null || menuActive != menuPause)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
                EventSystem.current.SetSelectedGameObject(firstButtonMenu);
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

    
    public void ToggleSlot(int index)
    {
        if (slotINDEX == index)
        {
            //Deselect if same key is pressed again
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
        //menuActive = null;
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

    public void ShowBossHealthBar(IBoss boss)
    {
        if (bossHealthBar != null)
        {
            bossHealthBar.SetActive(true);
            bossHealthBar.GetComponent<BossHealthBar>().AssignBoss(boss);
        }
    }

    public void SaveGame()
    {
        List<string> gunNames = new List<string>();

        foreach (gunStats gun in playerScript.arsenal)
        {
            gunNames.Add(gun.GunName);
        }

        string listItems = string.Join(",", gunNames);
        PlayerPrefs.SetString("OwnedWeapons", listItems);
        PlayerPrefs.Save();
    }

    public List<gunStats> LoadGame()
    {
        List<gunStats> loadedGuns = new List<gunStats>();

        string listItems = PlayerPrefs.GetString("OwnedWeapons", "");
        if (string.IsNullOrEmpty(listItems)) return loadedGuns;

        string[] gunNames = listItems.Split(',');

        foreach (string name in gunNames)
        {
            gunStats gun =  allGuns.Find(g => g.GunName == name);
            if(gun != null)
            {
                loadedGuns.Add(gun);
            }
        }
        return loadedGuns;
    }

    public void SetObjective(string Objective)
    {
        if(objectiveText != null)
        {
            objectiveText.text = "Current Objective: " + Objective;
        }
    }
    public IEnumerator UpdateElectricIcon(int _cooldown)
    {
        abilityOne.color = new Color(0.588f, 0.588f, 0.588f, abilityOne.color.a);
        yield return new WaitForSeconds(_cooldown);
        abilityOne.color = abilityOneColorOrig;
    }

    public IEnumerator UpdateFreezeIcon(int _cooldown)
    {
        abilityTwo.color = new Color(0.588f, 0.588f, 0.588f, abilityTwo.color.a);
        yield return new WaitForSeconds(_cooldown);
        abilityTwo.color= abilityTwoColorOrig;
    }


    public IEnumerator UpdateShockwaveIcon(int _cooldown)
    {
        abilityThree.color = new Color(0.588f, 0.588f, 0.588f, abilityThree.color.a);
        yield return new WaitForSeconds(_cooldown);
        abilityThree.color = abilityThreeColorOrig;
    }

}
