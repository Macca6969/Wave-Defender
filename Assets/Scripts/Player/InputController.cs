using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    [SerializeField] public bool InvertMouseY = false;

    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private WeaponSwitching weaponSwitching;


    [SerializeField] public Vector2 lookInput;
    [SerializeField] public Vector2 movementInput;
    [SerializeField] public float mouseScrollY;
 


    private void Awake()
    {
        playerInput = new PlayerInput();
        playerController = GetComponent<PlayerController>();
   
    }

    private void Update()
    {
        lookInput = playerInput.OnFoot.Look.ReadValue<Vector2>();
        movementInput = playerInput.OnFoot.Movement.ReadValue<Vector2>();

          if (mouseScrollY > 0)
        {
            Debug.Log("We scrolled up." + mouseScrollY);
            weaponSwitching.CheckForSwitch(mouseScrollY);
            mouseScrollY = 0f;
        }
        if (mouseScrollY < 0)
        {
            Debug.Log("We scrolled down." + mouseScrollY);
            weaponSwitching.CheckForSwitch(mouseScrollY);
            mouseScrollY = 0f;
        }


    }


    
    private void OnEnable()
    {
        playerInput.Enable();

 

        playerInput.OnFoot.Sprint.started += context => playerController.HandleSprinting(context);

        playerInput.OnFoot.Sprint.canceled += context => playerController.ResetMovementSpeed(context);

        playerInput.OnFoot.Jump.started += context => playerController.HandleJump(context);

        playerInput.OnFoot.Crouch.started += context => playerController.HandleCrouch(context);

        playerInput.OnFoot.Crouch.canceled += context => playerController.ResetMovementSpeed(context);

        playerInput.OnFoot.Crouch.canceled += context => playerController.ResetPlayerHeight(context);

        playerInput.OnFoot.Fire.started += context => playerController.PlayerFire(context);

        playerInput.OnFoot.Reload.started += context => playerController.PlayerReload(context);

        playerInput.OnFoot.ChangeWeapon.started += context => mouseScrollY = context.ReadValue<float>();
        

    
    }


    private void OnDisable()
    {
        playerInput.Disable();

    

        playerInput.OnFoot.Sprint.started -= context => playerController.HandleSprinting(context);

        playerInput.OnFoot.Sprint.canceled -= context => playerController.ResetMovementSpeed(context);

        playerInput.OnFoot.Jump.started -= context => playerController.HandleJump(context);

        playerInput.OnFoot.Crouch.started -= context => playerController.HandleCrouch(context);

        playerInput.OnFoot.Crouch.canceled -= context => playerController.ResetMovementSpeed(context);

        playerInput.OnFoot.Crouch.canceled -= context => playerController.ResetPlayerHeight(context);

        playerInput.OnFoot.Fire.started -= context => playerController.PlayerFire(context);

        playerInput.OnFoot.Reload.started -= context => playerController.PlayerReload(context);

        playerInput.OnFoot.ChangeWeapon.started -= context => mouseScrollY = context.ReadValue<float>();

    }

}