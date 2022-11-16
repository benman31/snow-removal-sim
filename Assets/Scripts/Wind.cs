using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    [SerializeField]
    private float windStrength;
    private Vector2 oldWindDir, currentWindDir, newWindDir;
    private float windDirectionTimeSlot, windDirectionCountDown = 0;
    private ParticleSystem particleSys;
    private ParticleSystem.VelocityOverLifetimeModule particleVelOverLifeTime;
    private ParticleSystem.MinMaxCurve minMaxX, minMaxY, minMaxZ;
    // Start is called before the first frame update
    void Start()
    {
        particleSys = GetComponent<ParticleSystem>();
        particleVelOverLifeTime = particleSys.velocityOverLifetime;

        //particleVelOverLifeTime.y = new ParticleSystem.MinMaxCurve(-1, -3);

        currentWindDir = new Vector2(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (windDirectionCountDown <= 0)
        {
            windDirectionTimeSlot = Random.Range(5, 20);
            windDirectionCountDown = windDirectionTimeSlot;

            newWindDir = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
            oldWindDir = currentWindDir;
        }

        currentWindDir = Vector2.Lerp(newWindDir, oldWindDir, windDirectionCountDown/windDirectionTimeSlot);

        minMaxX = new ParticleSystem.MinMaxCurve(currentWindDir.x, currentWindDir.x * windStrength);
        minMaxZ = new ParticleSystem.MinMaxCurve(currentWindDir.y, currentWindDir.y * windStrength); //wind direction is 2d so y is z in this case

        particleVelOverLifeTime.x = minMaxX;
        particleVelOverLifeTime.z = minMaxZ;

        windDirectionCountDown -= Time.deltaTime;

        Debug.Log("The current wind direction is " + currentWindDir + " The new wind direction is " + newWindDir + " the wind direction countdown is " + windDirectionCountDown);
    }
}
