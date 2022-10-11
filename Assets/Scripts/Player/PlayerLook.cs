using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
  private InputController input;
  //public Camera cam;
  public GameObject wep;
  public Vector2 lookInput;
  public float xSensitivity = 30f;
  public float ySensitivity = 30f;
  float mouseX, mouseY;
  private float xRotation = 0f;

  

    // Update is called once per frame
    void Update()
    {
        Look();
    }

    void Awake()
    {

        input = GetComponent<InputController>();
        Cursor.lockState = CursorLockMode.Locked;
    
    }

        private void Look()
    {
         //lookInput

         lookInput = input.lookInput;
         float mouseX = lookInput.x;
         float mouseY = lookInput.y;

         xRotation -= (mouseY * ySensitivity) / 10;
         xRotation = Mathf.Clamp(xRotation, -80f, 80);

        wep.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * (mouseX  * xSensitivity) / 10);
    }
}
