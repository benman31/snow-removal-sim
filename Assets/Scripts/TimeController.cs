/*
Written by: Abdelrahman Awad
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeController : MonoBehaviour
{

    [SerializeField] private Light sunLight;
    [SerializeField] private float maxSunLightIntesity;

    [SerializeField] private Light moonLight;
    [SerializeField] private float maxMoonLightIntesity;

    [SerializeField] private Color dayTimeAmbient;
    [SerializeField] private Color nightTimeAmbient;

    [SerializeField] private AnimationCurve lightChangeCurve;

    [SerializeField] private float timeMultiplier;

    [SerializeField] private float startHour;
    [SerializeField] private float sunriseHour;
    [SerializeField] private float sunsetHour;

    private DateTime currentTime;
    private TimeSpan sunriseTime;
    private TimeSpan sunsetTime;

    // Start is called before the first frame update
    void Start()
    {
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);

        sunriseTime = TimeSpan.FromHours(sunriseHour);
        sunsetTime = TimeSpan.FromHours(sunsetHour);
    }

    // Update is called once per frame
    void Update()
    {
        currentTime = currentTime.AddSeconds(Time.deltaTime * timeMultiplier);  

        RotateSun();
        UpdateLighting();
    }

    private void RotateSun()
    {
        float sunLightRotation;

        if (currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime)
        {
            TimeSpan dayTimeDuration = CalculateTimeDifference(sunriseTime, sunsetTime);
            TimeSpan timeSinceSunrise = CalculateTimeDifference(sunriseTime, currentTime.TimeOfDay);

            double dayTimeProportion = timeSinceSunrise.TotalMinutes / dayTimeDuration.TotalMinutes;

            sunLightRotation = Mathf.Lerp(0, 180, (float)dayTimeProportion);
        }
        else
        {
            TimeSpan nightTimeDuration = CalculateTimeDifference(sunsetTime, sunriseTime);
            TimeSpan timeSinceSunset = CalculateTimeDifference(sunsetTime, currentTime.TimeOfDay);

            double nightTimeProportion = timeSinceSunset.TotalMinutes / nightTimeDuration.TotalMinutes;

            sunLightRotation = Mathf.Lerp(180, 360, (float)nightTimeProportion);
        }

        //Debug.Log("the sun rotation is " + sunLight.transform.rotation);
        sunLight.transform.rotation = Quaternion.AngleAxis(sunLightRotation, Vector3.right);
    }

    private void UpdateLighting()
    {
        float dotProduct = Vector3.Dot(sunLight.transform.forward, Vector3.down);

        //make it scale from 0 to 1
        dotProduct += 1;
        dotProduct /= 2;

        sunLight.intensity = Mathf.Lerp(0, maxSunLightIntesity, dotProduct);
        moonLight.intensity = Mathf.Lerp(maxMoonLightIntesity, 0, dotProduct);
        // moonLight.intensity = 0;

        //Debug.Log("the dot product is " + dotProduct + " the sun intesity is " + sunLight.intensity + " the moon light intesity is " + moonLight.intensity);

        RenderSettings.ambientLight = Color.Lerp(nightTimeAmbient, dayTimeAmbient, dotProduct);

        // sunLight.intensity = Mathf.Lerp(0, maxSunLightIntesity, lightChangeCurve.Evaluate(dotProduct));
        // moonLight.intensity = Mathf.Lerp(maxMoonLightIntesity, 0, lightChangeCurve.Evaluate(dotProduct));

        // RenderSettings.ambientLight = Color.Lerp(nightTimeAmbient, dayTimeAmbient, lightChangeCurve.Evaluate(dotProduct));
    }

    private TimeSpan CalculateTimeDifference(TimeSpan fromtime, TimeSpan toTime)
    {
        TimeSpan difference = toTime - fromtime;

        if (difference.TotalSeconds < 0)
        {
            difference += TimeSpan.FromHours(24);
        }

        return difference;
    }
}
