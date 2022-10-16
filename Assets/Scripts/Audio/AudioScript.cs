using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AudioScript : NetworkBehaviour
{
  

  [Header("Player Sounds")]
  [SerializeField] public AudioSource walkingConcrete;
  [SerializeField] public AudioSource playerHealed;
  [SerializeField] public AudioSource playerLevelUp;
  [SerializeField] public AudioSource playerRevived;

  [Header("Pistol")]
  [SerializeField] public AudioSource pistolShoot;
  [SerializeField] public AudioSource pistolReload;
  [SerializeField] public AudioSource pistolEmpty;
  [SerializeField] public AudioSource pistolSelect;

  [Header("Rifle")]
  [SerializeField] public AudioSource rifleShoot;
  [SerializeField] public AudioSource rifleReload;
  [SerializeField] public AudioSource rifleEmpty;
  [SerializeField] public AudioSource rifleSelect;

  [Header("Heavy")]
  [SerializeField] public AudioSource heavyShoot;
  [SerializeField] public AudioSource heavyReload;
  [SerializeField] public AudioSource heavyEmpty;
  [SerializeField] public AudioSource heavySelect;

  [Header("Enemy")]
  [SerializeField] public AudioSource enemyDie;

}
