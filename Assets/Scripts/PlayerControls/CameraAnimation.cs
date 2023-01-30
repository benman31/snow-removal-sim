using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimation : MonoBehaviour
{
    public Camera cam;
    public float cameraTransitionTime = 4.0f;
    public bool goDown;
    [HideInInspector] public float timer = 0;

    private Quaternion originalRot;
    private Quaternion destRot = Quaternion.Euler(53.0f, 0f, 0f);

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //transition camera from current rotation to look downwards or vice versa


        if (goDown)
        {
            if (timer <= 1)
            {
                cam.transform.localRotation = Quaternion.Lerp(originalRot, destRot, timer);
            }

            timer += Time.deltaTime;

            if (timer >= 2.2f)
            {
                goDown = false;
                timer = 1.0f;
            }
        }
        else
        {
            cam.transform.localRotation = Quaternion.Lerp(originalRot, destRot, timer);
            timer -= Time.deltaTime;
        }

        Debug.Log("timer is " + timer);
    }

    public void SetOriginalRot(Quaternion rot)
    {
        originalRot = rot;
    }

    public void init()
    {
        originalRot = cam.transform.localRotation;
        timer = 0;
        goDown = true;
    }
}
