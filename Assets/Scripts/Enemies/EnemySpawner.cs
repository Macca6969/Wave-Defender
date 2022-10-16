using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EnemySpawner : NetworkBehaviour
{

    public Transform spawnPos;
    public GameObject enemy;
    public bool hasSpawned = false;
    public int enemyCount;
    private float x;
    private float z;
    Vector3 spawnPosRandom = new Vector3(0.1f, 0.1f, 0.1f);

private void Update() 
{
    if (isServer && enemyCount <= 5)
    {
    SpawnEnemy();
    }
}


void SpawnEnemy()
{

       spawnPos.position += new Vector3(Random.Range(-10, 10),
                           Random.Range(-0, 0),
                           Random.Range(-10, 10)
                           );
   
        GameObject enemySpawning = Instantiate(enemy, spawnPos.position, Quaternion.identity);
        Debug.Log("Spawning enemy.");
        NetworkServer.Spawn(enemySpawning);
        enemyCount++;
     
    
}



public void EnemyResetBool()
{
    hasSpawned = false;
}



}
