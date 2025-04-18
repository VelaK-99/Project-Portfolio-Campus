using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class gameManager : MonoBehaviour
{

    public static gameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    public GameObject enemyCountTextObject;
    public GameObject reloadGunText;
    public GameObject reloadingGun;

    [SerializeField] GameObject hotkeyBAR;

    public GameObject[] InventorySLOTS;
    public int slotINDEX = -1;

    public GameObject textActive;

    public GameObject player;
    public PlayerScript playerScript;
    public Image playerHPBar;
    public GameObject playerDamageScreen;
    

    public bool isPaused;

    float timeScaleOrig;

    public int currentRoom = 1;

    float gameGoalCount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerScript>();
        timeScaleOrig = Time.timeScale;
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
            }
        }

        if (Input.GetButtonDown("TAB"))
        {
            if (menuActive == null)
            {
                menuActive = hotkeyBAR;
                menuActive.SetActive(true);
            }
            else if (menuActive == hotkeyBAR)
            {
                menuActive.SetActive(false);
                menuActive = null;
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
    void ToggleSlot(int index)
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
            slotINDEX = index;
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

    public void UpdateGameGoal(int amount)
    {
        gameGoalCount += amount;

        if(gameGoalCount <= 0)
        {
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    } //Updates the game goal count when an enemy is spawned/killed

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
}
