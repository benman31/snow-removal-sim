/*
Written by: Abdelrahman Awad
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float sensitivity = 100f;

    public float xRotation;
    public float yRotation;

    private bool snowblowerActive = false;

    public bool on = true;

    public Transform playerBody;
    public Transform playerHands;
    [HideInInspector] public int equippedWeapon = 1;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        PlayerTools.OnSnowblowerActive += HandleSnowblowerActive;
    }

    private void OnDestroy()
    {
        PlayerTools.OnSnowblowerActive -= HandleSnowblowerActive;
    }

    // Update is called once per frame
    void Update()
    {
        if (on)
        {
            float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            if (snowblowerActive)
            {
                yRotation += mouseX;
                yRotation = Mathf.Clamp(yRotation, -75f, 75f);
            }
            else
            {
                yRotation = 0f;
            }
            

            //rotate camera vertically (and horizontally if snowblower is active)
            transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

            //rotate Hands vertically together with camera when flamethrower is equipped
            if(equippedWeapon == 2)
            {
                playerHands.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            }
            

            //rotate player horizontally is snow blower is not active
            if (!snowblowerActive)
                playerBody.Rotate(Vector3.up * mouseX);
        }

    }

    private void HandleSnowblowerActive(bool isActive)
    {
        snowblowerActive = isActive;
    }
}
