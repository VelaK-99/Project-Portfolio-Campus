using UnityEngine;

public class SetSceneObjective : MonoBehaviour
{
    [TextArea]
    public string Objective;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerScript>())
        {
            gameManager.instance.SetObjective(Objective);
            Destroy(gameObject);
        }
    }
}
