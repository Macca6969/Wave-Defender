using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
 

 #region Singleton

 public GameObject player;   //Change this to player once spawned.

public static PlayerManager instance;

private void Awake() 
{
    instance = this;
}



 #endregion
}
