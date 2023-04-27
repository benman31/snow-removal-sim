/*
Written by: Abdelrahman Awad
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherController : MonoBehaviour
{
    public static event Action<float> OnSnowRateChange;

    //Weather
    public enum Weather { Blizzard, Snowy, Clear };
    public Weather currentWeather = Weather.Clear;
    [SerializeField] private float weatherChangeDelay = 10.0f;
    private bool weatherUpdated = false;

    //Particles
    [HideInInspector] public ParticleSystem[] particleSystems;
    private ParticleSystem.EmissionModule[] emissionRates;

    //Post processing effects
    [Range(0.0f, 10.0f)] public float snowFallRate = 10.0f;
    [Range(0.0f, 10.0f)] public float meltingRate = 5.0f;
    public float meltDelay = 2;
    [Range(0.0f, 1.0f)] public float minimumVisibilityRadius;
    [Range(0.5f, 2.0f)] public float maxFrostIntesity;
    [Range(1.0f, 200.0f)] public float maxFrostAccumuationTime;
    [Range(1.0f, 200.0f)] public float minFrostAccumuationTime;

    [SerializeField] private Camera playerCam;
    private float frostIntensity = 0;
    private float distort = 0;
    private float dropletsSpeed;
    private bool firstMelt = false;

    //WindZone
    [SerializeField] private WindZone windZone;
    [SerializeField] private CTI.CTI_CustomWind ctiWind;

    //Wind
    [HideInInspector] public Wind wind;
    [HideInInspector] public float dotP;
    private const float MAXEMISSIONRATE = 1000.0F;
    private const float MAXWINDINTENSITY = 200.0F;
    [Range(1.0f, MAXWINDINTENSITY)] public float windIntesity = 100;
    public float windDirectionTimeSlotMin, windDirectionTimeSlotMax;

    private Vector2 windDirection;

    //Misc
    [SerializeField] private GameObject player;

    // Start is called before the first frame update
    void Awake()
    {
        //init
        particleSystems = new ParticleSystem[6];
        emissionRates = new ParticleSystem.EmissionModule[6];

        wind = GetComponentInChildren<Wind>();
        wind.windDirectionTimeSlotMin = windDirectionTimeSlotMin;
        wind.windDirectionTimeSlotMax = windDirectionTimeSlotMax;

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
        if (!weatherUpdated)
        {
            StartCoroutine(UpdateWeather());
            weatherUpdated = true;
        }

        //adjust particle system position to follow player & adjust emission rates depending on wind direction
        {
            int i = 0;
            foreach (Transform child in transform)
            {
                if (particleSystems[i].gameObject.name.Equals("Particle System up"))
                {
                    particleSystems[i].transform.position = player.transform.position + new Vector3(0, 10.0f, 0);
                    emissionRates[i].rateOverTime = Mathf.SmoothStep(0, MAXEMISSIONRATE/2, snowFallRate / 10);
                }
                else if (particleSystems[i].gameObject.name.Equals("Particle System left"))
                {
                    particleSystems[i].transform.position = player.transform.position + new Vector3(-25.0f, 7.5f, 0);

                    //only emit particles when wind direction is towards positive X axis
                    if (wind.currentWindDir.x > 0)
                    {
                        if (wind.currentWindDir.y != 0)
                        {
                            emissionRates[i].rateOverTime = Mathf.SmoothStep(0, MAXEMISSIONRATE, snowFallRate / 10) * Mathf.Lerp(0, 1, Mathf.Abs(wind.currentWindDir.x / wind.currentWindDir.y));
                        }
                        else
                        {
                            emissionRates[i].rateOverTime = Mathf.SmoothStep(0, MAXEMISSIONRATE, snowFallRate / 10);
                        }
                    }
                    else
                    {
                        emissionRates[i].rateOverTime = 0;
                    }

                }
                else if (particleSystems[i].gameObject.name.Equals("Particle System right"))
                {
                    particleSystems[i].transform.position = player.transform.position + new Vector3(25.0f, 7.5f, 0);

                    if (wind.currentWindDir.x < 0)
                    {
                        if (wind.currentWindDir.y != 0)
                        {
                            emissionRates[i].rateOverTime = Mathf.SmoothStep(0, MAXEMISSIONRATE, snowFallRate / 10) * Mathf.Lerp(0, 1, Mathf.Abs(wind.currentWindDir.x / wind.currentWindDir.y));
                        }
                        else
                        {
                            emissionRates[i].rateOverTime = Mathf.SmoothStep(0, MAXEMISSIONRATE, snowFallRate / 10);
                        }
                    }
                    else
                    {
                        emissionRates[i].rateOverTime = 0;
                    }
                }
                else if (particleSystems[i].gameObject.name.Equals("Particle System forward"))
                {
                    particleSystems[i].transform.position = player.transform.position + new Vector3(0, 7.5f, 25.0f);

                    if (wind.currentWindDir.y < 0)
                    {
                        if (wind.currentWindDir.x != 0)
                        {
                            emissionRates[i].rateOverTime = Mathf.SmoothStep(0, MAXEMISSIONRATE, snowFallRate / 10) * Mathf.Lerp(0, 1, Mathf.Abs(wind.currentWindDir.y / wind.currentWindDir.x));
                        }
                        else
                        {
                            emissionRates[i].rateOverTime = Mathf.SmoothStep(0, MAXEMISSIONRATE, snowFallRate / 10);
                        }
                    }
                    else
                    {
                        emissionRates[i].rateOverTime = 0;
                    }
                }
                else if (particleSystems[i].gameObject.name.Equals("Particle System backward"))
                {
                    particleSystems[i].transform.position = player.transform.position + new Vector3(0, 7.5f, -25.0f);

                    if (wind.currentWindDir.y > 0)
                    {
                        if (wind.currentWindDir.x != 0)
                        {
                            emissionRates[i].rateOverTime = Mathf.SmoothStep(0, MAXEMISSIONRATE, snowFallRate / 10) * Mathf.Lerp(0, 1, Mathf.Abs(wind.currentWindDir.y/wind.currentWindDir.x));
                        }
                        else
                        {
                            emissionRates[i].rateOverTime = Mathf.SmoothStep(0, MAXEMISSIONRATE, snowFallRate / 10);
                        }
                    }
                    else
                    {
                        emissionRates[i].rateOverTime = 0;
                    }
                }
                else if (particleSystems[i].gameObject.name.Equals("Particle System global"))
                {
                    particleSystems[i].transform.position = player.transform.position + new Vector3(0, 30.0f, 0);
                    emissionRates[5].rateOverTime = Mathf.SmoothStep(0, MAXEMISSIONRATE/2, snowFallRate / 10);
                }

                i++;
            }
        }

        //wind zone
        ctiWind.WindDirection = new Vector3(wind.currentWindDir.x, 0, wind.currentWindDir.y);
        windZone.windMain = Mathf.Lerp(0, 3.0f, windIntesity / MAXWINDINTENSITY);

        //wind script
        wind.windIntesity = windIntesity;

        // for (int i = 0; i < 5; i++)
        // {
        //     emissionRates[i].rateOverTime = Mathf.SmoothStep(0, 195, snowFallRate / 10);
        // }

        // //global particle system in the sky
        // emissionRates[5].rateOverTime = Mathf.SmoothStep(0, 195, snowFallRate / 10);

        StartCoroutine(PPEffects());
    }

    IEnumerator UpdateWeather()
    {
        yield return new WaitForSeconds(weatherChangeDelay);

        if (currentWeather == Weather.Blizzard)
        {
            windIntesity = UnityEngine.Random.Range(150, 201);
            snowFallRate = UnityEngine.Random.Range(8, 11);
        }
        else if (currentWeather == Weather.Snowy)
        {
            windIntesity = UnityEngine.Random.Range(75, 151);
            snowFallRate = UnityEngine.Random.Range(3, 8);
        }
        else
        {
            windIntesity = UnityEngine.Random.Range(1, 75);
            snowFallRate = 0;
        }
        OnSnowRateChange?.Invoke(snowFallRate);
        weatherUpdated = false;
    }

    IEnumerator PPEffects()
    {
        Vector2 windDir = wind.currentWindDir;
        dotP = Vector3.Dot(playerCam.transform.forward, new Vector3(windDir.x, 0, windDir.y));

        //Debug.Log("the dot product is  " + dotP);

        if (dotP < 0)
        {
            playerCam.GetComponent<PostProcessingCamera>().radius = Mathf.Lerp(1.0f, minimumVisibilityRadius, frostIntensity);

            playerCam.GetComponent<PostProcessingCamera>().feather = Mathf.Lerp(1.5f, 0.3f, frostIntensity);

            playerCam.GetComponent<PostProcessingCamera>().Intensity = Mathf.Lerp(0.5f, maxFrostIntesity, frostIntensity);

            if (frostIntensity < 1)
            {
                frostIntensity += (Time.deltaTime * (-1 * dotP)) / Mathf.SmoothStep(maxFrostAccumuationTime, minFrostAccumuationTime, windIntesity / 200);
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
            dropletsSpeed = Mathf.SmoothStep(1, 4, frostIntensity / 1.0f);
        }

        playerCam.GetComponent<PostProcessingCamera>().distortion = Mathf.Lerp(0, 5.0f, distort);
    }
}
