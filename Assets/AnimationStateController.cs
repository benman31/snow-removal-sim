using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    Animator animator;
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
                animator.SetBool("isDigging", true);
                animator.SetBool("haveSnow", true);
            }
        }
    }
}
