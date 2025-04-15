using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    [SerializeField] GameObject Player;
    [SerializeField] int test;

    private void LateUpdate()
    {
        transform.position = new Vector3(Player.transform.position.x,test,Player.transform.position.z);
    }
}
