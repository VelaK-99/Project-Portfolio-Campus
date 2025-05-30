using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    private GameObject Player;



    private void Start()
    {
        Player = gameManager.instance.player;
    }
    private void LateUpdate()
    {
       if(Player) transform.position = new Vector3(Player.transform.position.x,40,Player.transform.position.z);
    }
}
