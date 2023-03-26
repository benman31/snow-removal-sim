/*
Written by: Abdelrahman Awad
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationStateController : MonoBehaviour
{
    public Transform SnowPoofTransform;
    public GameObject SnowPoof;
    public Animator animator;
    [SerializeField] private Image poiseBar;
    [Range(0, 100.0f)] public float poise;

    [SerializeField] private GameObject[] snowPrefabs;
    private int activatedPrefab = -1;
    private int currentWeapon = 1; //1 for shovel

    private CameraAnimation camAnim;

    private const float MAXPOISE = 100.0f;
    private const float POISEACCUMULATIONRATE = 90.0f;

    private bool isDigging;
    private bool carryingSnow;

    // These states are used purely for digging, not animation related
    private bool makingHole = false;
    private bool droppingSnow = false;
    private MouseLook mouseLook;


    // Start is called before the first frame update
    void Start()
    {
        camAnim = this.GetComponentInChildren<CameraAnimation>();
        animator = this.GetComponent<Animator>();
        mouseLook = this.GetComponentInChildren<MouseLook>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && currentWeapon == 1)
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

        if (Input.GetKeyDown(KeyCode.Alpha3) && !isDigging && !carryingSnow)
        {
            if (currentWeapon == 1)
            {

                animator.SetInteger("currentWep", 2);
            }
            else
            {
                animator.SetInteger("currentWep", 1);
            }

            // mouseLook.equippedWeapon = currentWeapon;

            // animator.SetInteger("currentWep", currentWeapon);
        }

        //Debug.Log(currentWeapon);

        if (isDigging)
        {
            poise += POISEACCUMULATIONRATE * Time.deltaTime;
            poiseBar.fillAmount = poise / MAXPOISE;
        }

        if (isDigging && Input.GetMouseButtonUp(0))
        {
            isDigging = false;
        }
    }

    IEnumerator PlayCameraShovelAnimation()
    {
        //disable camera mouse controls and player movement controls
        DisablePlayerControls();

        camAnim.SetDestRot(Quaternion.Euler(53.0f, 0f, 0f));
        camAnim.InitShovelling();
        camAnim.enabled = true;

        yield return new WaitForSeconds(camAnim.cameraTransitionTime);

        //enable controls again
        EnablePlayerControls();

        camAnim.enabled = false;
    }

    IEnumerator PlayCamWepSwitchAnim()
    {
        //disable camera mouse controls and player movement controls
        DisablePlayerControls();

        camAnim.SetDestRot(new Quaternion(0,0,0,1));
        camAnim.InitWepSwitch();
        camAnim.enabled = true;

        yield return new WaitForSeconds(camAnim.cameraTransitionTime);

        camAnim.enabled = false;
    }

    private void DisablePlayerControls()
    {
        GetComponent<PlayerMovement>().enabled = false;
        GetComponentInChildren<MouseLook>().enabled = false;
    }

    private void EnablePlayerControls()
    {
        GetComponent<PlayerMovement>().enabled = true;
        GetComponentInChildren<MouseLook>().enabled = true;
    }

    private void SpawnSnowOnShovel()
    {
        if (poise <= 30)
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
    }

    private void DestroySnowOnShovel()
    {
        if (activatedPrefab != -1)
        {
            snowPrefabs[activatedPrefab].SetActive(false);
        }
    }

    private void StartDigging()
    {
        isDigging = true;
    }

    private void StopDigging()
    {
        isDigging = false;
    }

    private void EquipFlameThrower()
    {
        currentWeapon = 2;
        mouseLook.equippedWeapon = currentWeapon;
    }

    private void EquipShovel()
    {
        currentWeapon = 1;
        mouseLook.equippedWeapon = currentWeapon;
    }

    private void SpawnSnowParticles()
    {
        GameObject trail = Instantiate(SnowPoof, new Vector3(SnowPoofTransform.position.x, 0, SnowPoofTransform.position.z), SnowPoof.transform.rotation);
    }

    private void CarrySnow()
    {
        carryingSnow = true;
    }

    private void DropSnow()
    {
        carryingSnow = false;
    }
    // Ben Code -- used for shovel digging state (no animation)
    private void MakeHole()
    {
        makingHole = true;
    }

    private void StopMakingHole()
    {
        makingHole = false;
    }

    private void DumpSnow()
    {
        droppingSnow = true;
    }

    private void StopDumpingSnow()
    {
        droppingSnow = false;
    }
    // End Ben Code
    private void ResetPoise()
    {
        poise = 0;
        poiseBar.fillAmount = poise / MAXPOISE;
    }

    public bool IsPlayerDigging()
    {
        return isDigging;
    }

    public bool IsCarryingSnow()
    {
        return carryingSnow;
    }
    // Ben Code used for shovel digging state (no animation)
    public bool IsMakingHole()
    {
        return makingHole;
    }

    public bool IsDroppingSnow()
    {
        return droppingSnow;
    }
    // End Ben Code
}
