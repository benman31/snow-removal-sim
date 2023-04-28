/*
Written by: Abdelrahman Awad
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimation : MonoBehaviour
{
    public Camera cam;
    public float cameraTransitionTime = 4.0f;
    public bool shovelling = false;
    public bool switchingWeapons = false;
    [HideInInspector] public float timer = 0;

    [SerializeField] private Transform playerHandsTrans;
    private Quaternion originalRot;
    private Quaternion destRot = Quaternion.Euler(53.0f, 0f, 0f);
    

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //transition camera from current rotation to new rotation
        if(switchingWeapons)
        {
            if (timer <= 1)
            {
                cam.transform.localRotation = Quaternion.Lerp(originalRot, destRot, timer);
                playerHandsTrans.localRotation = Quaternion.Lerp(originalRot, destRot, timer);
            }

            timer += Time.deltaTime*3;

            if (timer >= 100f)
            {
                switchingWeapons = false;
                timer = 1.0f;
            }
        }
        else if (shovelling)
        {
            if (timer <= 1)
            {
                cam.transform.localRotation = Quaternion.Lerp(originalRot, destRot, timer);
            }

            timer += Time.deltaTime;

            if (timer >= 2.2f)
            {
                shovelling = false;
                timer = 1.0f;
            }
        }
        else
        {
            cam.transform.localRotation = Quaternion.Lerp(originalRot, destRot, timer);
            timer -= Time.deltaTime;
        }

        Debug.Log("dest rot is  " + destRot + "original ");
    }

    public void SetDestRot(Quaternion rot)
    {
        destRot = rot;
    }

    public void SetOriginalRot(Quaternion rot)
    {
        originalRot = rot;
    }

    public void InitShovelling()
    {
        originalRot = cam.transform.localRotation;
        timer = 0;
        shovelling = true;
    }

    public void InitWepSwitch()
    {
        originalRot = cam.transform.localRotation;
        timer = 0;
        switchingWeapons = true;
    }
}
