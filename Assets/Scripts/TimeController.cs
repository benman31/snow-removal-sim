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

    [SerializeField] private AnimationCurve shadowStrengthCurve;
    [SerializeField] private AnimationCurve lightChangeCurve;

    private float timeMultiplier;
    private float startHour;
    private float sunriseHour = 6.5f;
    private float sunsetHour = 18.5f;

    private DateTime currentTime;
    private TimeSpan sunriseTime;
    private TimeSpan sunsetTime;

    // Start is called before the first frame update
    void Awake()
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

        sunLight.transform.rotation = Quaternion.AngleAxis(sunLightRotation, Vector3.right);
    }

    private void UpdateLighting()
    {
        float dotProduct = Vector3.Dot(sunLight.transform.forward, Vector3.down);

        //make it scale from 0 to 1
        dotProduct += 1;
        dotProduct /= 2;

        // sunLight.intensity = Mathf.Lerp(0.4f, maxSunLightIntesity, lightChangeCurve.Evaluate(dotProduct));
        // moonLight.intensity = Mathf.SmoothStep(maxMoonLightIntesity, 0, lightChangeCurve.Evaluate(dotProduct));

        sunLight.intensity = Mathf.Lerp(0f, maxSunLightIntesity, dotProduct);
        moonLight.intensity = Mathf.SmoothStep(maxMoonLightIntesity, 0, dotProduct);

        moonLight.shadowStrength = Mathf.SmoothStep(maxMoonLightIntesity, 0, dotProduct);
        sunLight.shadowStrength = Mathf.Lerp(0, maxSunLightIntesity, shadowStrengthCurve.Evaluate(dotProduct));

        RenderSettings.ambientLight = Color.Lerp(nightTimeAmbient, dayTimeAmbient, dotProduct);
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

    public void SetStartHour(float value)
    {
        startHour = value;
    }

    public void SetSunRiseHour(float value)
    {
        sunriseHour = value;
    }

    public void SetSunSetHour(float value)
    {
        sunsetHour = value;
    }

    public void setTimeMultiplier(float value)
    {
        timeMultiplier = value;
    }
}
