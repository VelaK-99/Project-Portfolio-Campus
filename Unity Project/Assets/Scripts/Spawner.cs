using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    [SerializeField] GameObject[] objectsToSpawn;
    [SerializeField] int numToSpawn;
    [SerializeField] int timeBetweenSpawns;
    [SerializeField] Transform[] spawnPos;
    [SerializeField] OpenDoor entrance;
    [SerializeField] OpenDoor exit;    
    private bool isCleared = false;

    public List<GameObject> spawnList = new List<GameObject>();



    float spawnTimer;
    int spawnCount;
    bool startSpawning;
    
    void Start()
    {
        gameManager.instance.CountSpawner();

        if (entrance == null)
        {
            entrance = transform.parent.Find("entrance")?.GetComponent<OpenDoor>();
        }
        if (exit == null)
        {
            entrance = transform.parent.Find("exit")?.GetComponent<OpenDoor>();
        }
    }
    
    void Update()
    {
        /*
        if (startSpawning)
        {
            spawnTimer += Time.deltaTime;

            if (spawnCount < numToSpawn && spawnTimer >= timeBetweenSpawns)
            {
                spawn();
            }
        }
        */
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startSpawning = true;
            entrance.LockDoor();
            exit.LockDoor();
        }
    }
    /*
    void spawn()
    {
        int arrayPos = Random.Range(0, spawnPos.Length);
        int enemyArrayIndex = Random.Range(0, objectsToSpawn.Length);

        Vector3 basePos = spawnPos[arrayPos].position;
        Vector3 offset = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));

        Vector3 finalPos = basePos + offset;

        GameObject enemyClone = Instantiate(objectsToSpawn[enemyArrayIndex], finalPos, spawnPos[arrayPos].rotation);
        spawnList.Add(enemyClone);

        gameManager.instance.AddEnemy(enemyClone);        
        spawnCount++;
        spawnTimer = 0;
    }
    */
    public void checkEnemyTotal()
    {

        
        if (spawnList.Count <= 0 && !isCleared)
        {          
            
            entrance.UnlockDoor();
            exit.UnlockDoor();

            isCleared = true;
            

            gameManager.instance.ClearSpawners();
            gameManager.instance.UpdateRoom();

            Destroy(this);
        }
    }
}
