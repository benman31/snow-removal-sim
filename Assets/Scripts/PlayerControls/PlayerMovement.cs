/*
Written by: Abdelrahman Awad
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    
    public Transform groundCheck;
    public LayerMask groundMask;
    float sphereRad = 0.4f;
    
    public Vector3 velocity;
    public float speed = 10f;
    public float jumpHeight = 1f;
    float gravity = -9.81f * 2;

    bool isGrounded = true;
    const float YVELOCITYRESET = -2F;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, sphereRad, groundMask);

        /**
        MOVEMENT
        */

        //Debug.Log(isGrounded);

        if(isGrounded && velocity.y < 0.001f)
        {
            velocity.y = YVELOCITYRESET;
        } 

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 movement = transform.right * x + transform.forward * z;

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
}

