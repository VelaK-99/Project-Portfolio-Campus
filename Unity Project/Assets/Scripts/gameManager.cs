using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
using UnityEngine.EventSystems;
=======
using UnityEngine.UI;
using TMPro;
>>>>>>> Stashed changes
=======
using UnityEngine.UI;
using TMPro;
>>>>>>> Stashed changes
=======
using UnityEngine.UI;
using TMPro;
>>>>>>> Stashed changes

public class gameManager : MonoBehaviour
{

    public static gameManager instance;

    public int currentRoom = 0;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream

    [SerializeField] GameObject menuBAR;

    public GameObject[] InventorySLOTS;
    public int slotINDEX = -1;

=======
    [SerializeField] TMP_Text enemyGoalCountText;
>>>>>>> Stashed changes
=======
    [SerializeField] TMP_Text enemyGoalCountText;
>>>>>>> Stashed changes
=======
    [SerializeField] TMP_Text enemyGoalCountText;
>>>>>>> Stashed changes
    public GameObject textActive;

    public GameObject player;
    public PlayerScript playerScript;
    public GameObject playerDamageScreen;
    public Image playerHPBar;

    public bool isPaused;

    float timeScaleOrig;

    public float gameGoalCount;
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
                menuActive = menuBAR;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuBAR)
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
        enemyGoalCountText.text = gameGoalCount.ToString("F0");

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

}
