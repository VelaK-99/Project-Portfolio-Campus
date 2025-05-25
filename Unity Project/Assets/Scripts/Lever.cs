using UnityEngine;

public class Lever : MonoBehaviour, IInteract
{
    public bool isActivated = false;
    public PuzzleManager puzzleManager;

    [SerializeField] private Transform leverSwitch; 
    [SerializeField] private Vector3 downRotation;  
    [SerializeField] private Vector3 upRotation;    

    void Start()
    {
        if (leverSwitch == null)
            leverSwitch = transform; 

        upRotation = leverSwitch.localEulerAngles; 
    }

    public void Interact()
    {
        isActivated = !isActivated;

        if (isActivated)
        {
            leverSwitch.localEulerAngles = downRotation; 
        }
        else
        {
            leverSwitch.localEulerAngles = upRotation;   
        }

        puzzleManager.CheckLevers();
    }
}