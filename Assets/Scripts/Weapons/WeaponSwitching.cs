using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    
    public PlayerController playerController;

    private  int selectedWeapon2;

    
    private void Start()
     {
        selectedWeapon2 = 0;
        playerController.CmdSelectedWeapon(selectedWeapon2);
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
        }
    }


}
