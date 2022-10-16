using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Enemy : NetworkBehaviour
{

    [SerializeField] private int setEnemyHealth = 100;
    [SerializeField] private int setEnemyXp = 600;

    [SyncVar(hook = nameof(EnemyUpdateHealth))]
    public int enemyHealth = 100;

    public AudioScript audioScript;


    private void Start()
    {
        enemyHealth = setEnemyHealth;

    }

    void EnemyUpdateHealth(int oldValue, int newValue)
    {
        Debug.Log("Enemy Health is " + enemyHealth);
    }

    public void TakeDamage(int damageAmount, string playerName)
    {
        enemyHealth -= damageAmount;
        Debug.Log("Enemy now has " + enemyHealth + " health remaining.");
        if (enemyHealth <= 0)
        {
            int enemyXp = setEnemyXp;
            CmdEnemyDie(enemyXp, playerName);
            audioScript.enemyDie.Play();
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdEnemyDie(int enemyXp, string playerName)
    {
        if (isServer)
        {
            Debug.Log("Enemy Dead.");
            Destroy(gameObject);
            CmdGivePlayerXp(enemyXp, playerName);


            string spawner = "GameManager";
            GameObject gameManager = GameObject.Find(spawner);
            EnemySpawner enemySpawner = gameManager.GetComponent<EnemySpawner>();
            enemySpawner.enemyCount -= 1;
        }
    }


    [Command(requiresAuthority = false)]
    void CmdGivePlayerXp(int enemyXp, string playerName)
    {
        GameObject player = GameObject.Find(playerName);
        LevelScript levelScript = player.GetComponent<LevelScript>();
        levelScript.RpcPlayerGainExperience(enemyXp, playerName);


    }

}
