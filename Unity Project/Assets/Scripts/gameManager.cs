using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class gameManager : MonoBehaviour
{

    public static gameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    [SerializeField] GameObject menuBAR;

    public GameObject[] InventorySLOTS;

    public GameObject textActive;

    public GameObject player;
    public PlayerScript playerScript;

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
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("num1"))
        {
            EventSystem.current.SetSelectedGameObject(InventorySLOTS[0]);
        }       
        
        if (Input.GetButtonDown("num2"))
        {
            EventSystem.current.SetSelectedGameObject(InventorySLOTS[1]);
        }

        if (Input.GetButtonDown("num3"))
        {
            EventSystem.current.SetSelectedGameObject(InventorySLOTS[2]);
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

}
