using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Enemy : NetworkBehaviour
{
   
   [SyncVar(hook = nameof(EnemyUpdateHealth))]
   int enemyHealth = 100;



   public void EnemyUpdateHealth()
   {
       
   }

   public void TakeDamage(int damageAmount)
   {
         enemyHealth -= damageAmount;
   }
}
