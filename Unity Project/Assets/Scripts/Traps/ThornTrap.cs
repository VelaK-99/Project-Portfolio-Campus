using System.Collections;
using UnityEngine;

public class ThornTrap : MonoBehaviour
{
    public Vector3 moveOffset = new Vector3(0, 2, 0);
    public float moveDuration = 0.5f; 
    public float interval = 5f;


    private Vector3 startPos;
    private Vector3 targetPos;


    private bool isMovingUp;
    

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + moveOffset;
        StartCoroutine(MoveCycle());
    }

    IEnumerator MoveCycle()
    {
        while (true)
        {
          
            yield return StartCoroutine(MoveSpike(startPos, targetPos));
            yield return new WaitForSeconds(0.5f); 

         
            yield return StartCoroutine(MoveSpike(targetPos, startPos));
            yield return new WaitForSeconds(interval); 
        }
    }

    
    IEnumerator MoveSpike(Vector3 from, Vector3 to)
    {
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(from, to, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = to;
    }

  
}
