/*
Written by: Abdelrahman Awad
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    //Weather
    public enum Weather { Blizzard, Snowy, Clear };
    [HideInInspector] public Weather currentWeather = Weather.Clear;

    //Wind
    [HideInInspector] public Vector2 oldWindDir, currentWindDir, newWindDir;
 
    [HideInInspector] public float windIntesity, windDirectionTimeSlotMin, windDirectionTimeSlotMax;
    private float windDirectionTimeSlot, windDirectionCountDown = 0;

    //Particle System
    private ParticleSystem.VelocityOverLifetimeModule particleVelOverLifeTime1, particleVelOverLifeTime2, particleVelOverLifeTime3, particleVelOverLifeTime4, particleVelOverLifeTime5;
    private ParticleSystem.MinMaxCurve minMaxX, minMaxY, minMaxZ;

    // Start is called before the first frame update
    void Start()
    {
        particleVelOverLifeTime1 = gameObject.GetComponentInParent<WeatherController>().particleSystems[0].velocityOverLifetime;
        particleVelOverLifeTime2 = gameObject.GetComponentInParent<WeatherController>().particleSystems[1].velocityOverLifetime;
        particleVelOverLifeTime3 = gameObject.GetComponentInParent<WeatherController>().particleSystems[2].velocityOverLifetime;
        particleVelOverLifeTime4 = gameObject.GetComponentInParent<WeatherController>().particleSystems[3].velocityOverLifetime;
        particleVelOverLifeTime5 = gameObject.GetComponentInParent<WeatherController>().particleSystems[4].velocityOverLifetime;

        currentWindDir = new Vector2(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (windDirectionCountDown <= 0)
        {
            windDirectionTimeSlot = Random.Range(windDirectionTimeSlotMin, windDirectionTimeSlotMax);
            windDirectionCountDown = windDirectionTimeSlot;

            // newWindDir = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
            // oldWindDir = currentWindDir;

            newWindDir = new Vector2(1,0);
            oldWindDir = newWindDir;
        }

        currentWindDir = Vector2.Lerp(oldWindDir, newWindDir, windDirectionCountDown/windDirectionTimeSlot);

        minMaxX = new ParticleSystem.MinMaxCurve(currentWindDir.x, currentWindDir.x * windIntesity);
        minMaxZ = new ParticleSystem.MinMaxCurve(currentWindDir.y, currentWindDir.y * windIntesity); //wind direction is 2d so y is z in this case

        particleVelOverLifeTime1.x = minMaxX;
        particleVelOverLifeTime1.z = minMaxZ;

        particleVelOverLifeTime2.y = new ParticleSystem.MinMaxCurve(-currentWindDir.x, -currentWindDir.x * windIntesity);
        particleVelOverLifeTime2.z = minMaxZ;

        particleVelOverLifeTime3.y = new ParticleSystem.MinMaxCurve(-currentWindDir.x, -currentWindDir.x * windIntesity);
        particleVelOverLifeTime3.z = minMaxZ;

        particleVelOverLifeTime4.z = minMaxX;
        particleVelOverLifeTime4.y = minMaxZ;

        particleVelOverLifeTime5.z = minMaxX;
        particleVelOverLifeTime5.y = minMaxZ;

        windDirectionCountDown -= Time.deltaTime;

        //Debug.Log("The current wind direction is " + currentWindDir + " The new wind direction is " + newWindDir + " the wind direction countdown is " + windDirectionCountDown);
    }
}
