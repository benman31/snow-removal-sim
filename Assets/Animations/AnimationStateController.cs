/*
Written by: Abdelrahman Awad
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    public Animator animator;

    private CameraAnimation camAnim;

    // Start is called before the first frame update
    void Start()
    {
        camAnim = this.GetComponentInChildren<CameraAnimation>();
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
                //StartCoroutine(PlayCameraAnimation());
    
                animator.SetBool("isDigging", true);
                animator.SetBool("haveSnow", true);
            }
        }
    }

    IEnumerator PlayCameraAnimation()
    {
        //disable camera mouse controls and player movement controls
        GetComponent<PlayerMovement>().enabled = false;
        GetComponentInChildren<MouseLook>().enabled = false; 

        camAnim.init();
        camAnim.enabled = true;

        yield return new WaitForSeconds(camAnim.cameraTransitionTime);

        //enable controls again
        GetComponent<PlayerMovement>().enabled = true;
        GetComponentInChildren<MouseLook>().enabled = true;

        camAnim.enabled = false;
    }
}
