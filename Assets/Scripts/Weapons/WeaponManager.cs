using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WeaponManager : NetworkBehaviour
{



    [Header("Scripts.")]
    public Player player;
    public PlayerController playerController;
    public AudioScript audioScript;
    public ManageUI manageUI;
    public Pistol pistol;
    public Rifle rifle;
    public Heavy heavy;


    [Header("Objects")]
    public Camera cam;
    public Camera shootingCam;

    

    public bool isFiring;




    public void PlayerCheckWeapon(int selectedWeapon, bool playerShooting)
    {
        if (selectedWeapon == 0)
        {
            pistol.FiringPistol(playerShooting);
            isFiring = false;
        }

        if (selectedWeapon == 1)
        {
            rifle.FiringRifle(playerShooting);
            isFiring = false;
        }

        if (selectedWeapon == 2)
        {
            heavy.FiringHeavy(playerShooting);
            isFiring = false;
        }

    }

}







