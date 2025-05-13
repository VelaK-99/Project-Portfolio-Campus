using UnityEngine;

public class cameraController : MonoBehaviour
{

    [SerializeField] int sens;
    [SerializeField] int lockVertMax, lockVertMin;
    [SerializeField] bool invertY;

    float rotX;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //initialize rotateX to match the camera's initial X rotation
        rotX = transform.localEulerAngles.x;
        
        //Convert to range -180 to 180 to match clamping behavior
        if (rotX > 180)
        {
            rotX -= 360;
        }

    }


    // Update is called once per frame
    void Update()
    {
        //get input
        float mouseX = Input.GetAxis("Mouse X") * sens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sens * Time.deltaTime;

        if (invertY)
            rotX += mouseY;
        else
            rotX -= mouseY;

        //clamp the camera on the x-axis
        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

        //rotate the camera on the x-axis (up and down)
        transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        //rotate the player on his y-axis (left and right)
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
