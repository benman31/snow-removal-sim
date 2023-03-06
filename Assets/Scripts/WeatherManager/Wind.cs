/*
Written by: Abdelrahman Awad
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    //Wind
    [HideInInspector] public Vector2 oldWindDir, currentWindDir, newWindDir;
 
    [HideInInspector] public float windIntesity, windDirectionTimeSlotMin, windDirectionTimeSlotMax;
    private float windDirectionTimeSlot, windDirectionCountDown = 0;

    //Particle System
    private ParticleSystem.VelocityOverLifetimeModule particleVelOverLifeTime1, particleVelOverLifeTime2, particleVelOverLifeTime3, particleVelOverLifeTime4, particleVelOverLifeTime5, particleVelOverLifeTime6;
    private ParticleSystem.MinMaxCurve minMaxX, minMaxY, minMaxZ;

    // Start is called before the first frame update
    void Start()
    {
        particleVelOverLifeTime1 = gameObject.GetComponentInParent<WeatherController>().particleSystems[0].velocityOverLifetime;
        particleVelOverLifeTime2 = gameObject.GetComponentInParent<WeatherController>().particleSystems[1].velocityOverLifetime;
        particleVelOverLifeTime3 = gameObject.GetComponentInParent<WeatherController>().particleSystems[2].velocityOverLifetime;
        particleVelOverLifeTime4 = gameObject.GetComponentInParent<WeatherController>().particleSystems[3].velocityOverLifetime;
        particleVelOverLifeTime5 = gameObject.GetComponentInParent<WeatherController>().particleSystems[4].velocityOverLifetime;
        particleVelOverLifeTime6 = gameObject.GetComponentInParent<WeatherController>().particleSystems[5].velocityOverLifetime;

        currentWindDir = new Vector2(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (windDirectionCountDown <= 0)
        {
            windDirectionTimeSlot = Random.Range(windDirectionTimeSlotMin, windDirectionTimeSlotMax + 0.1f);
            windDirectionCountDown = windDirectionTimeSlot;

            // newWindDir = new Vector2(Random.Range(-1.0f, 1.1f), Random.Range(-1.0f, 1.1f));
            // oldWindDir = currentWindDir;

            newWindDir = new Vector2(-0.3f,-.1f);
            oldWindDir = newWindDir;
        }

        currentWindDir = Vector2.Lerp(oldWindDir, newWindDir, windDirectionCountDown/windDirectionTimeSlot);

        minMaxX = new ParticleSystem.MinMaxCurve(currentWindDir.x * (windIntesity/2.0f), currentWindDir.x * windIntesity);
        minMaxZ = new ParticleSystem.MinMaxCurve(currentWindDir.y * (windIntesity/2.0f), currentWindDir.y * windIntesity); //wind direction is 2d so y is z in this case

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

        //recalc to make global particle system out of sync from other particle systems for more snow variety
        minMaxX = new ParticleSystem.MinMaxCurve(currentWindDir.x * (windIntesity/16.0f), currentWindDir.x * (windIntesity/8.0f));
        minMaxZ = new ParticleSystem.MinMaxCurve(currentWindDir.y * (windIntesity/16.0f), currentWindDir.y * (windIntesity/8.0f)); //wind direction is 2d so y is z in this case

        particleVelOverLifeTime6.x = minMaxX;
        particleVelOverLifeTime6.z = minMaxZ;

        windDirectionCountDown -= Time.deltaTime;

        //Debug.Log("The current wind direction is " + currentWindDir + " The new wind direction is " + newWindDir + " the wind direction countdown is " + windDirectionCountDown);
    }
}
