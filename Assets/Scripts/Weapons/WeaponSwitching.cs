using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    
    public PlayerController playerController;


     private  int selectedWeapon2;
     [SerializeField] private Camera pistolCamera;
     [SerializeField] private Camera rifleCamera;
     [SerializeField] private Camera heavyCamera;

    private void Start()
     {
        selectedWeapon2 = 0;
        playerController.CmdSelectedWeapon(selectedWeapon2);
        //WeaponCamera();
     }


    public void ChangeWeapon(float mouseScrollY)
    {

        int previousSelectedWeapon2 = selectedWeapon2;

        if (mouseScrollY > 0f)
        {
            if (selectedWeapon2 >= transform.childCount - 1)
                selectedWeapon2 = 0;
            else
                selectedWeapon2++;
        }



        if (mouseScrollY < 0f)
        {
            if (selectedWeapon2 <= 0)
                selectedWeapon2 = transform.childCount - 1;
            else
                selectedWeapon2--;
        }
        if (previousSelectedWeapon2 != selectedWeapon2)
        {
            playerController.CmdSelectedWeapon(selectedWeapon2);
            //WeaponCamera();
        }
    }

    /*private void WeaponCamera()
    {
        if (selectedWeapon2 == 0)
        {
           pistolCamera.enabled = true;
           rifleCamera.enabled = false;
           heavyCamera.enabled = false;
        }
        if (selectedWeapon2 == 1)
        {
           pistolCamera.enabled = false;
           rifleCamera.enabled = true;
           heavyCamera.enabled = false;
        }
        if (selectedWeapon2 == 2)
        {
           pistolCamera.enabled = false;
           rifleCamera.enabled = false;
           heavyCamera.enabled = true;
        }
    }
    */
}
