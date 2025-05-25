using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public Lever[] levers;
    public GameObject door;

    public void CheckLevers()
    {
        foreach (Lever lever in levers)
        {
            if (!lever.isActivated)
                return; // If any lever is not active, exit
        }

        OpenDoor();
    }

    void OpenDoor()
    {
        if (door != null)
            door.SetActive(false); // Or trigger an animation, etc.
    }
}
