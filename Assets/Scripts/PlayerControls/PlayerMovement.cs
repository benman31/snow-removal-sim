/*
Written by: Abdelrahman Awad
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static event Action<bool> OnPlayerGrounded;

    public CharacterController controller;

    public Transform groundCheck;
    public LayerMask groundMask;

    public Vector3 velocity;
    public Vector3 movement;
    public float speed = 10f;
    public float snowBlowerMovementSpeed = 5f;
    public float snowblowerRotationSpeed = 20f;
    public float jumpHeight = 1f;
    public float gravity = -9.81f * 2;


    private bool isGrounded = true;
    private const float YVELOCITYRESET = -2F;
    private float sphereRad = 0.4f;

    private bool snowblowerActive;

    // Start is called before the first frame update
    void Start()
    {
        PlayerTools.OnSnowblowerActive += HandleSnowblowerActive;
    }

    private void OnDestroy()
    {
        PlayerTools.OnSnowblowerActive -= HandleSnowblowerActive;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, sphereRad, groundMask);
        OnPlayerGrounded?.Invoke(isGrounded);
        if (!snowblowerActive)
        {
            HandlePlayerInput();
        }
        else
        {
            HandleSnowBlowerInput();
        }
    }

    // Input Code by Awad, Moved into HandleInput function by Ben
    private void HandlePlayerInput()
    {
        /**
        MOVEMENT
        */

        //Debug.Log(isGrounded);

        if (isGrounded && velocity.y < 0.001f)
        {
            velocity.y = YVELOCITYRESET;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
 
        movement = transform.right * x + transform.forward * z;
        
        controller.Move(movement * speed * Time.deltaTime);

        /**
        Jumping
        */

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Debug.Log("I am jumping");
            velocity.y = Mathf.Sqrt(jumpHeight * YVELOCITYRESET * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    // Change the controls to typical tank controls
    private void HandleSnowBlowerInput()
    {

        if (isGrounded && velocity.y < 0.001f)
        {
            velocity.y = YVELOCITYRESET;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        movement = transform.forward * z;

        controller.Move(movement * snowBlowerMovementSpeed * Time.deltaTime);

        Quaternion desiredRotation = transform.rotation * Quaternion.Euler(Vector3.up * snowblowerRotationSpeed * x * Mathf.Sign(z) * Time.deltaTime);
        transform.rotation = desiredRotation;

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

    }

    private void HandleSnowblowerActive(bool isActive)
    {
        snowblowerActive = isActive;
    }
}

