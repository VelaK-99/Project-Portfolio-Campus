using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;

public class RoomManager : MonoBehaviour
{
    public List<GameObject> enemies;
    public GameObject wallToDestroy;

    private int remainingEnemies;

    private bool roomActivated = false;
    void Start()
    {
        remainingEnemies = enemies.Count;

        foreach (var enemy in enemies)
        {
            enemy.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (roomActivated || other.CompareTag("Player"))
        {
            return;
        }
        roomActivated = true;

        foreach (var enemy in enemies)
        {
            enemy.SetActive(true);
        }        

        GetComponent<Collider>().enabled = false;
    }

    public void OnEnemyKilled()
    {

        remainingEnemies--;

        if(remainingEnemies <= 0)
        {
            wallToDestroy.SetActive(false);
            gameManager.instance.currentRoom++;
        }
    }
}
