/*
Written by: Abdelrahman Awad
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    public Animator animator;

    public Camera cam;

    private float cameraTransitionTime;
    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (animator.GetBool("isDigging"))
            {
                animator.SetBool("isDigging", false);
                animator.SetBool("haveSnow", false);
            }
            else
            {
                cam.GetComponent<MouseLook>().enabled = false;

                animator.SetBool("isDigging", true);
                animator.SetBool("haveSnow", true);

                cameraTransitionTime = 1.0f;
                //make player look down
                PlayerLookDown(cam.transform.rotation.eulerAngles);
                cam.transform.Rotate(Vector3.forward, 45);

                cam.GetComponent<MouseLook>().enabled = true;
            }
        }
    }

    void PlayerLookDown(Vector3 oldRot)
    {
        Vector3 newRot = new Vector3(28.5f, 0,0);

        float timer = 0;

        Debug.Log("cam angles are " + cam.transform.rotation.eulerAngles);
        while(cam.transform.rotation.eulerAngles != newRot)
        {
            cam.transform.rotation = Quaternion.Euler(Vector3.Lerp(oldRot, newRot, timer/cameraTransitionTime));
            
            timer += Time.deltaTime;
        }
    }
}
