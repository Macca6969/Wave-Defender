using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Mirror;

[RequireComponent(typeof(Player))]
public class PlayerSetupScript : NetworkBehaviour
{



public InputController inputController;
public PlayerInput playerInput;
public PlayerController playerController;
public PlayerLook playerLook;
public Camera cam;
public Camera wepCam;
public GameObject playerUI;
public Player playerScript;
public SetupUI setupUI;
public WeaponSwitching weaponSwitching;
public Rigidbody rb;
public CharacterController characterController;


private void Start() 
{
GetComponent<Player>().SetupPlayer();

}


public override void OnStartClient()
{
    base.OnStartClient();

    string _netID = GetComponent<NetworkIdentity>().netId.ToString();
    Player _player = GetComponent<Player>();

    GameManager.RegisterPlayer(_netID, _player);
}


public override void OnStartLocalPlayer()
{

//disable by default on player prefab
  
  inputController.enabled = true;
  playerController.enabled = true;
  playerLook.enabled = true;
  weaponSwitching.enabled = true;
  cam.gameObject.SetActive(true);
  wepCam.gameObject.SetActive(true);
  playerUI.gameObject.SetActive(true);
  setupUI.SetupPlayerUI();

}

private void OnDisable()
{
    GameManager.UnRegisterPlayer (transform.name);
}


}

