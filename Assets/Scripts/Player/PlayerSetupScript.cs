using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Mirror;

[RequireComponent(typeof(Player))]
public class PlayerSetupScript : NetworkBehaviour
{
    [Header("Scripts")]
    public ManageUI setupUI;
    public PlayerLook playerLook;
    public PlayerController playerController;
    public InputController inputController;
    public PlayerInput playerInput;
    public WeaponSwitching weaponSwitching;
    public Player playerScript;
    public AudioScript audioScript;
    public LevelScript levelScript;
    public PlayerHealth playerHealth;
    public Heavy heavy;
    public Rifle rifle;
    public Pistol pistol;
    public WeaponManager weaponManager;

    [Header("Objects")]
    public Camera cam;
    public Camera wepCam;
    public Camera shootingCam;
    public GameObject playerUI;
    public Rigidbody rb;
    public CharacterController characterController;
    public GameObject rightHand;
    public GameObject player;
    public GameObject playerItems;




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
        audioScript.enabled = true;
        levelScript.enabled = true;
        playerHealth.enabled = true;
        setupUI.enabled = true;
        heavy.enabled = true;
        rifle.enabled = true;
        pistol.enabled = true;
        weaponManager.enabled = true;

        cam.gameObject.SetActive(true);
        cam.tag = "LocalPlayer";

        wepCam.gameObject.SetActive(true);
        shootingCam.gameObject.SetActive(true);
        
        playerUI.gameObject.SetActive(true);
        setupUI.SetupPlayerUI();

        GetComponent<PlayerHealth>().SetupPlayerHealth();

        int LayerPlayerWeapons = LayerMask.NameToLayer("PlayerWeapons");
        int Invisible = LayerMask.NameToLayer("Invisible");
        bool includeInactive = true;

        foreach (Transform t in rightHand.GetComponentsInChildren<Transform>(includeInactive))
        {
            t.gameObject.layer = LayerPlayerWeapons;
        }

         foreach (Transform t in playerItems.GetComponentsInChildren<Transform>(includeInactive))
        {
            t.gameObject.layer = Invisible;
        }


        player.layer = Invisible;


 
       
        
        Debug.Log("Current layer: " + rightHand.layer);



    }

    private void OnDisable()
    {
        GameManager.UnRegisterPlayer(transform.name);
    }


}

