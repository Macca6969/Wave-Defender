using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Animations;
using TMPro;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    private InputController input;


    [Header("Misc")]
    public static PlayerController instance;
    private CharacterController controller;
    public Player player;
    public AudioScript audioScript;
    public ManageUI manageUI;


    [Header("Movement")]
    [SerializeField] public float movementSpeed = default;
    [SerializeField] public bool isMoving;
    [SerializeField] private Vector3 movementInputDirection;
    [SerializeField] private float smoothMovementInputSpeed = 0.1f;
    public Vector3 movementInput;
    private Vector2 currentMovementInputVector;
    private Vector2 smoothMovementInputVelocity;
    [SerializeField] private float currentSpeed;

    [Header("Sprinting")]
    [SerializeField] public float walkSpeed = default;
    [SerializeField] public float sprintSpeed = default;
    [SerializeField] private bool isSprinting;

    [Header("Crouching / Standing")]
    [SerializeField] public float crouchSpeed = default;
    [SerializeField] private bool isCrouching;
    [SerializeField] private float crouchHeight = default;
    [SerializeField] private float standingHeight = default;

    [Header("Gravity / GroundCheck")]
    [SerializeField] private Vector3 playerVelocity;
    public float gravity = -9.81f;
    [SerializeField] public bool isGrounded;
    public LayerMask Ground;
    [SerializeField] private float distToGround = 2f;

    [Header("Falling")]
    [SerializeField] private float currentFallSpeed = default;
    [SerializeField] private bool isFalling;

    [Header("Jumping")]
    [SerializeField] private float jumpHeight = default;
    [SerializeField] public bool isJumping;



    [Header("Weapons")]
    [SyncVar(hook = nameof(SelectWeapon))]
    public int selectedWeapon = 0;
    public WeaponSwitching weaponSwitching;
    public GameObject weaponPistol;
    public GameObject weaponRifle;
    public GameObject weaponHeavy;
    public WeaponManager weaponManager;
    [SerializeField] public bool playerShooting;

    void Awake()
    {
        instance = this;
        input = GetComponent<InputController>();
        controller = GetComponent<CharacterController>();
    }


    // Update is called once per frame
    void Update()
    {
        HandleMovementInput();
        GroundCheck();
        Gravity();
        HandleFalling();
    }


    private void HandleMovementInput()
    {
        movementInput = input.movementInput;
        movementInput.Normalize();
        currentMovementInputVector = Vector2.SmoothDamp(currentMovementInputVector, movementInput, ref smoothMovementInputVelocity, smoothMovementInputSpeed);
        movementInputDirection = transform.forward * currentMovementInputVector.y + transform.right * currentMovementInputVector.x;

        controller.Move(movementInputDirection * movementSpeed * Time.deltaTime);
        controller.Move(playerVelocity * Time.deltaTime);

        playerVelocity.y += Physics.gravity.y * Time.deltaTime;

        if (movementInput != Vector3.zero)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }

    private void GroundCheck()
    {
        if (isCrouching)
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, distToGround / 2 + 0.1f);
        }
        else
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, distToGround + 0.1f);
        }
    }


    private void Gravity()
    {
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
            isFalling = false;
        }
        playerVelocity.y += gravity * Time.deltaTime;
    }

    public void ResetMovementSpeed(InputAction.CallbackContext context)
    {
        movementSpeed = walkSpeed;
        isCrouching = false;
        isSprinting = false;
    }

    public void HandleJump(InputAction.CallbackContext context)
    {
        if (!isJumping && isGrounded)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
            isJumping = true;
        }
    }

    public void HandleSprinting(InputAction.CallbackContext context)
    {
        context.ReadValueAsButton();
        if (!isSprinting && isGrounded)
        {
            movementSpeed = sprintSpeed;
            isSprinting = true;
        }
    }

    public void HandleCrouch(InputAction.CallbackContext context)
    {
        context.ReadValueAsButton();
        if (!isCrouching && isGrounded)
        {
            movementSpeed = crouchSpeed;
            isCrouching = true;
            controller.height = crouchHeight;
        }
    }

    public void ResetPlayerHeight(InputAction.CallbackContext context)
    {
        movementSpeed = walkSpeed;
        isCrouching = false;
        controller.height = standingHeight;
    }

    public void HandleFalling()
    {
        currentFallSpeed = playerVelocity.y;
        if (!isGrounded && currentFallSpeed < -3f)
        {
            isFalling = true;
            isJumping = false;
        }
        else
        {
            isFalling = false;
        }
    }

    public void PlayerFire(InputAction.CallbackContext context)
    {
        playerShooting = true;
        weaponManager.PlayerCheckWeapon(selectedWeapon, playerShooting);
    }

    public void PlayerFireStop(InputAction.CallbackContext context)
    {
        playerShooting = false;
    }

    public void PlayerReload(InputAction.CallbackContext context)
    {
        if (selectedWeapon == 0)
        {
            weaponManager.pistol.PistolReload();
        }

        if (selectedWeapon == 1)
        {
            weaponManager.rifle.RifleReload();
        }

        if (selectedWeapon == 2)
        {
            weaponManager.heavy.HeavyReload();
        }
    }


    [Command(requiresAuthority = false)]
    public void CmdSelectedWeapon(int selectedWeapon2)
    {
        selectedWeapon = selectedWeapon2;
    }

    public void SelectWeapon(int weaponOld, int weaponNew)
    {
        if (selectedWeapon == 0)
        {
            weaponPistol.SetActive(true);
            weaponRifle.SetActive(false);
            weaponHeavy.SetActive(false);
            audioScript.pistolSelect.Play();
            manageUI.UpdateAmmoUI();
        }

        if (selectedWeapon == 1)
        {
            weaponPistol.SetActive(false);
            weaponRifle.SetActive(true);
            weaponHeavy.SetActive(false);
            audioScript.rifleSelect.Play();
            manageUI.UpdateAmmoUI();
        }

        if (selectedWeapon == 2)
        {
            weaponPistol.SetActive(false);
            weaponRifle.SetActive(false);
            weaponHeavy.SetActive(true);
            audioScript.heavySelect.Play();
            manageUI.UpdateAmmoUI();
        }
    }
        
}




