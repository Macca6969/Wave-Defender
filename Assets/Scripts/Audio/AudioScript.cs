using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AudioScript : NetworkBehaviour
{
  
    [SerializeField] public AudioSource playerHitSound;
    [SerializeField] public AudioSource pistolReload;
    [SerializeField] public AudioSource pistolEmpty; 
    [SerializeField] public AudioSource pistolShot;

}
