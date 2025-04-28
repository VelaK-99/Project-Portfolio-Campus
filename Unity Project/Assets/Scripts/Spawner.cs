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
    OpenDoor door;
    private bool isCleared = false;

    public List<GameObject> spawnList = new List<GameObject>();



    float spawnTimer;
    int spawnCount;
    bool startSpawning;
    
    void Start()
    {
        gameManager.instance.CountSpawner();
    }
    
    void Update()
    {
        if (startSpawning)
        {
            spawnTimer += Time.deltaTime;

            if (spawnCount < numToSpawn && spawnTimer >= timeBetweenSpawns)
            {
                spawn();
            }
        }
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

    void spawn()
    {
        int arrayPos = Random.Range(0, spawnPos.Length);
        int enemyArrayIndex = Random.Range(0, objectsToSpawn.Length);

        GameObject enemyClone = Instantiate(objectsToSpawn[enemyArrayIndex], spawnPos[arrayPos].position, spawnPos[arrayPos].rotation);
        spawnList.Add(enemyClone);

        gameManager.instance.AddEnemy(enemyClone);
        enemyClone.GetComponent<EnemyAI>().whereICameFrom = this;
        spawnCount++;
        spawnTimer = 0;
    }

    public void checkEnemyTotal()
    {
        if (spawnList.Count <= 0 && !isCleared)
        {
            entrance.UnlockDoor();
            exit.UnlockDoor();

            isCleared = true;

            gameManager.instance.ClearSpawners();
        }
    }
}
