using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EnemySpawner : NetworkBehaviour
{

    public Transform spawnPos;
    public GameObject enemy;
    public int enemyCount;
    private bool isSpawning = false;

    Vector3 spawnPosRandom;


    public override void OnStartServer()
    {
        RespawnEnemies();
    }

    public void RespawnEnemies()
    {

        StartCoroutine(SpawnEnemy());

    }

    IEnumerator SpawnEnemy()
    {

        if (!isSpawning)
        {
            if (isServer && enemyCount <= 5)
            {
                isSpawning = true;
                yield return new WaitForSeconds(1f);
                Vector3 spawnPosRandom = spawnPos.position + new Vector3(Random.Range(-30, 30), 0, Random.Range(-30, 30));
                GameObject enemySpawning = Instantiate(enemy, spawnPosRandom, Quaternion.identity);
                Debug.Log("Spawning enemy.");
                NetworkServer.Spawn(enemySpawning);
                enemyCount++;
                isSpawning = false;
                StartCoroutine(SpawnEnemy());
            }
        }
    }
}
