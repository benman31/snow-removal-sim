/*
Written by: Abdelrahman Awad
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherController : MonoBehaviour
{
    [HideInInspector] public ParticleSystem[] particleSystems;
    private ParticleSystem.EmissionModule[] emissionRates;

    //Post processing effects
    [SerializeField] private Camera playerCam;

    [Range(0.0f, 10.0f)] public float snowFallRate = 10.0f;
    [Range(0.0f, 10.0f)] public float meltingRate = 5.0f;
    public float meltDelay = 2;
    [Range(0.0f, 1.0f)] public float minimumVisibilityRadius;
    [Range(0.5f, 2.0f)] public float maxFrostIntesity;
    [Range(1.0f, 200.0f)] public float maxFrostAccumuationRate;
    [Range(1.0f, 200.0f)] public float minFrostAccumuationRate;
    private float frostIntensity = 0;
    private float distort = 0;
    private float dropletsSpeed;
    private bool firstMelt = false;

    //Wind
    [Range(1.0f, 200.0f)] public float windIntesity = 100;
    public float windDirectionTimeSlotMin, windDirectionTimeSlotMax;

    private Wind wind;
    private Vector2 windDirection;


    // Start is called before the first frame update
    void Start()
    {
        //init
        particleSystems = new ParticleSystem[5];
        emissionRates = new ParticleSystem.EmissionModule[5];

        wind = GetComponentInChildren<Wind>();

        int i = 0;

        foreach (Transform child in transform)
        {
            particleSystems[i] = child.gameObject.GetComponent<ParticleSystem>();
            emissionRates[i] = particleSystems[i].emission;
            i++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        wind.windIntesity = windIntesity;

        for (int i = 0; i < 5; i++)
        {
            emissionRates[i].rateOverTime = Mathf.SmoothStep(0, 195, snowFallRate / 10);
        }

        StartCoroutine(PPEffects());
    }

    IEnumerator PPEffects()
    {
        Vector2 windDir = wind.currentWindDir;
        float dotP = Vector3.Dot(playerCam.transform.forward, new Vector3(windDir.x, 0, windDir.y));

        Debug.Log("the dot product is  " + dotP);

        if (dotP < 0)
        {
            playerCam.GetComponent<PostProcessingCamera>().radius = Mathf.Lerp(1.0f, minimumVisibilityRadius, frostIntensity);

            playerCam.GetComponent<PostProcessingCamera>().feather = Mathf.Lerp(1.5f, 0.3f, frostIntensity);

            playerCam.GetComponent<PostProcessingCamera>().Intensity = Mathf.Lerp(0.5f, maxFrostIntesity, frostIntensity);

            if (frostIntensity < 1)
            {
                frostIntensity += (Time.deltaTime * (-1 * dotP)) / Mathf.SmoothStep(maxFrostAccumuationRate, minFrostAccumuationRate, windIntesity / 200);
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
                    playerCam.GetComponent<PostProcessingCamera>().dropletsSpeed = 2.0f;
                }

                else if (frostIntensity > 0.6f && frostIntensity < 0.8f)
                {
                    playerCam.GetComponent<PostProcessingCamera>().waterShaderPass = 1;
                    playerCam.GetComponent<PostProcessingCamera>().dropletsSpeed = 3.0f;
                }

                else if (frostIntensity > 0.8f)
                {
                    playerCam.GetComponent<PostProcessingCamera>().waterShaderPass = 2;
                    playerCam.GetComponent<PostProcessingCamera>().dropletsSpeed = 4.0f;
                }
            }

            yield return new WaitForSeconds(meltDelay); //wait 2 seconds before snow accumulation melts

            playerCam.GetComponent<PostProcessingCamera>().radius = Mathf.Lerp(1.0f, minimumVisibilityRadius, frostIntensity);

            playerCam.GetComponent<PostProcessingCamera>().feather = Mathf.Lerp(1.5f, 0.3f, frostIntensity);

            playerCam.GetComponent<PostProcessingCamera>().Intensity = Mathf.Lerp(0.5f, maxFrostIntesity, frostIntensity);

            if (frostIntensity >= 0)
            {
                frostIntensity -= Time.deltaTime / Mathf.SmoothStep(10, 1, meltingRate / 10);
            }
        }

        if (distort > 0)
        {
            distort -= Time.deltaTime / Mathf.SmoothStep(10, 1, meltingRate / 10);
            dropletsSpeed = Mathf.SmoothStep(1, 4, frostIntensity/1.0f);
        }

        playerCam.GetComponent<PostProcessingCamera>().distortion = Mathf.Lerp(0, 5.0f, distort);
    }
}
