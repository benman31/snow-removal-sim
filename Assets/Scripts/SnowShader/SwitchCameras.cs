/*
 * Author: Benjamin Enman, 97377
 */
using UnityEngine;

public class SwitchCameras : MonoBehaviour
{
    public Camera firstPersonCamera;
    public Camera overheadCamera;

    // Call this function to disable FPS camera,
    // and enable overhead camera.
    public void ShowOverheadView()
    {
        firstPersonCamera.enabled = false;
        overheadCamera.enabled = true;
    }

    // Call this function to enable FPS camera,
    // and disable overhead camera.
    public void ShowFirstPersonView()
    {
        firstPersonCamera.enabled = true;
        overheadCamera.enabled = false;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!firstPersonCamera.enabled)
            {
                this.ShowFirstPersonView();
            }
            else
            {
                this.ShowOverheadView();
            }
            
        }
        
    }
}