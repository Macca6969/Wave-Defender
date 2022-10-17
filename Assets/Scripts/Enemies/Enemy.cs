using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class Enemy : NetworkBehaviour
{

    [SerializeField] private int setEnemyHealth = 100;
    [SerializeField] private int setEnemyXp = 600;

    [SyncVar(hook = nameof(EnemyUpdateHealth))]
    public int enemyHealth = 100;

    public AudioScript audioScript;

    public GameObject damageTextPrefab;

    public float destroyTime = 3f;
    


    private void Start()
    {
        enemyHealth = setEnemyHealth;

    }

    public void EnemyUpdateHealth(int oldValue, int newValue)
    {
        Debug.Log("Enemy Health is " + enemyHealth);
        ServerShowDamage();
        //RpcShowDamageText();
       
    }

    [Command(requiresAuthority = false)]
    public void CmdTakeDamage(int damageAmount, string playerName)
    {
        string enemyName = gameObject.name;
        RpcTakeDamage(damageAmount, playerName, enemyName);

        //RpcShowDamageText();

    }


    [ClientRpc]
    public void RpcTakeDamage(int damageAmount, string playerName, string enemy)
    {
        if (gameObject.name == enemy)
        {
            //RpcShowDamageText();
            enemyHealth -= damageAmount;
            Debug.Log("Enemy now has " + enemyHealth + " health remaining.");
            if (enemyHealth <= 0)
            {
                int enemyXp = setEnemyXp;
                CmdEnemyDie(enemyXp, playerName);
                audioScript.enemyDie.Play();
            }
        }
    }

    [Server]
    public void ServerShowDamage()
    {
            GameObject damageTextSpawn = Instantiate(damageTextPrefab, transform.position, Quaternion.identity);
            damageTextSpawn.GetComponent<TMP_Text>().text = enemyHealth.ToString();
            NetworkServer.Spawn(damageTextSpawn);
             //RpcShowDamageText();
             Debug.Log("Server Show Damage");
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
            enemySpawner.RespawnEnemies();
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
