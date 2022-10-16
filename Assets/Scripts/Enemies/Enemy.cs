using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Enemy : NetworkBehaviour
{
   
   [SyncVar(hook = nameof(EnemyUpdateHealth))]
   int enemyHealth = 100;



   void EnemyUpdateHealth(int oldValue, int newValue)
   {
       Debug.Log("Enemy Health is " + enemyHealth);
   }

   public void TakeDamage(int damageAmount)
   {
         enemyHealth -= damageAmount;
   }
}
