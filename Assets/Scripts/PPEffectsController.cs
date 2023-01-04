/*
Written by: Abdelrahman Awad
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PPEffectsController : MonoBehaviour
{
    //Post processing effects
    [SerializeField] private Camera playerCam;

    public float accumulationRate = 5.0f;
    public float meltingRate = 10.0f;
    private float frostIntensity = 0;
    private float distort = 0;
    private bool firstMelt = false;

    //Wind
    private Wind wind;
    private Vector2 windDirection;

    // Start is called before the first frame update
    void Start()
    {
        wind = GetComponentInChildren<Wind>();
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(PPEffects());
    }

    IEnumerator PPEffects()
    {
        Vector2 windDir = wind.currentWindDir;
        Vector2 cameraFront = new Vector2(playerCam.transform.forward.x, playerCam.transform.forward.z);
        float dotP = Vector2.Dot(cameraFront, windDir);

        Debug.Log("the dot product is  " + dotP);

        if (dotP > 0)
        {
            playerCam.GetComponent<PostProcessingCamera>().radius = Mathf.Lerp(1.0f, 0.0f, frostIntensity);

            playerCam.GetComponent<PostProcessingCamera>().feather = Mathf.Lerp(1.5f, 0.3f, frostIntensity);

            playerCam.GetComponent<PostProcessingCamera>().Intensity = Mathf.Lerp(0.5f, 1.5f, frostIntensity);

            if (frostIntensity < 1)
            {
                frostIntensity += (Time.deltaTime * dotP) / accumulationRate;
            }

            if (!firstMelt && frostIntensity > 0.4f) // make sure droptlets dont render unless a minimum amount of frost accumulated on the screen
            {
                firstMelt = true;
            }
        }
        else
        {
            if (firstMelt) // adjust shader pass according to the amount of frost on the screen when melting starts after accumulating 
            {
                firstMelt = false;

                distort = frostIntensity;

                if (frostIntensity > 0.5f && frostIntensity < 0.6f)
                {
                    playerCam.GetComponent<PostProcessingCamera>().waterShaderPass = 0; // the higher the shader pass the more water droplets on the screen
                    playerCam.GetComponent<PostProcessingCamera>().timeScale = 2.0f;
                }

                else if (frostIntensity > 0.6f && frostIntensity < 0.8f)
                {
                    playerCam.GetComponent<PostProcessingCamera>().waterShaderPass = 1;
                    playerCam.GetComponent<PostProcessingCamera>().timeScale = 3.0f;
                }

                else if (frostIntensity > 0.8f)
                {
                    playerCam.GetComponent<PostProcessingCamera>().waterShaderPass = 2;
                    playerCam.GetComponent<PostProcessingCamera>().timeScale = 4.0f;
                }
            }

            yield return new WaitForSeconds(2); //wait 2 seconds before snow accumulation melts

            playerCam.GetComponent<PostProcessingCamera>().radius = Mathf.Lerp(1.0f, 0.0f, frostIntensity);

            playerCam.GetComponent<PostProcessingCamera>().feather = Mathf.Lerp(1.5f, 0.3f, frostIntensity);

            playerCam.GetComponent<PostProcessingCamera>().Intensity = Mathf.Lerp(0.5f, 1.5f, frostIntensity);

            if (frostIntensity >= 0)
            {
                frostIntensity -= Time.deltaTime / meltingRate;
            }
        }

        if (distort > 0)
        {
            distort -= Time.deltaTime / meltingRate;
        }

        playerCam.GetComponent<PostProcessingCamera>().distortion = Mathf.Lerp(0, 5.0f, distort);
    }
}
