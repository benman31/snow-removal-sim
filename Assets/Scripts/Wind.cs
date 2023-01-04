/*
Written by: Abdelrahman Awad
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    //Weather
    public enum Weather {Blizzard, Snowy, Clear};
    public Weather currentWeather = Weather.Clear;
    
    //Wind
    public Vector2 oldWindDir, currentWindDir, newWindDir;
    [SerializeField] private float windIntesity = 2, windDirectionTimeSlotMin, windDirectionTimeSlotMax;
    private float windDirectionTimeSlot, windDirectionCountDown = 0;

    //Particle System
    public ParticleSystem particleSys1, particleSys2, particleSys3, particleSys4, particleSys5;

    private ParticleSystem.VelocityOverLifetimeModule particleVelOverLifeTime1, particleVelOverLifeTime2, particleVelOverLifeTime3, particleVelOverLifeTime4, particleVelOverLifeTime5;
    private ParticleSystem.MinMaxCurve minMaxX, minMaxY, minMaxZ;
    // Start is called before the first frame update
    void Start()
    {
        particleVelOverLifeTime1 = particleSys1.velocityOverLifetime;
        particleVelOverLifeTime2 = particleSys2.velocityOverLifetime;
        particleVelOverLifeTime3 = particleSys3.velocityOverLifetime;
        particleVelOverLifeTime4 = particleSys4.velocityOverLifetime;
        particleVelOverLifeTime5 = particleSys5.velocityOverLifetime;

        //particleVelOverLifeTime.y = new ParticleSystem.MinMaxCurve(-1, -3);

        currentWindDir = new Vector2(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        // if (windDirectionCountDown <= 0)
        // {
        //     windDirectionTimeSlot = Random.Range(windDirectionTimeSlotMin, windDirectionTimeSlotMax);
        //     windDirectionCountDown = windDirectionTimeSlot;

        //     newWindDir = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
        //     oldWindDir = currentWindDir;
        // }

        // currentWindDir = Vector2.Lerp(oldWindDir, newWindDir, windDirectionCountDown/windDirectionTimeSlot);

        // minMaxX = new ParticleSystem.MinMaxCurve(currentWindDir.x, currentWindDir.x * windIntesity);
        // minMaxZ = new ParticleSystem.MinMaxCurve(currentWindDir.y, currentWindDir.y * windIntesity); //wind direction is 2d so y is z in this case

        // particleVelOverLifeTime1.x = minMaxX;
        // particleVelOverLifeTime1.z = minMaxZ;

        // particleVelOverLifeTime2.y = new ParticleSystem.MinMaxCurve(-currentWindDir.x, -currentWindDir.x * windIntesity);
        // particleVelOverLifeTime2.z = minMaxZ;

        // particleVelOverLifeTime3.y = new ParticleSystem.MinMaxCurve(-currentWindDir.x, -currentWindDir.x * windIntesity);
        // particleVelOverLifeTime3.z = minMaxZ;

        // particleVelOverLifeTime4.z = minMaxX;
        // particleVelOverLifeTime4.y = minMaxZ;

        // particleVelOverLifeTime5.z = minMaxX;
        // particleVelOverLifeTime5.y = minMaxZ;

        // windDirectionCountDown -= Time.deltaTime;

        currentWindDir = new Vector2(1,0);

        //Debug.Log("The current wind direction is " + currentWindDir + " The new wind direction is " + newWindDir + " the wind direction countdown is " + windDirectionCountDown);
    }
}
