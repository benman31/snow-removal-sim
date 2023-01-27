/*
Written by: Abdelrahman Awad
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    public Animator animator;

    [Range(0, 100.0f)] public float poise;

    private CameraAnimation camAnim;

    [SerializeField] private GameObject[] snowPrefabs;
    private int activatedPrefab = -1;


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

    IEnumerator SpawnSnowOnShovel()
    {
        if(poise <= 30)
        {
            snowPrefabs[0].SetActive(true);
            activatedPrefab = 0;
        }
        else if (poise <= 70)
        {
            snowPrefabs[1].SetActive(true);
            activatedPrefab = 1;
        }
        else
        {
            snowPrefabs[2].SetActive(true);
            activatedPrefab = 2;
        }

        yield return 0;
    }

    IEnumerator DestroySnowOnShovel()
    {
        if(activatedPrefab != -1)
        {
            snowPrefabs[activatedPrefab].SetActive(false);
        }
    
        yield return 0;
    }
}
