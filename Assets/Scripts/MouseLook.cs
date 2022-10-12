using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float sensitivity = 100f;

    float xRotation = 0f;

    public Transform playerBody;
    public Transform playerHands;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        //rotate camera vertically
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        //rotate Hands vertically together with camera
        playerHands.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        //rotate player horizontally
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
