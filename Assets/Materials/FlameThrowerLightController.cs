using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrowerLightController : MonoBehaviour
{
    [SerializeField] private Light lightSource;
    [SerializeField] private float minIntensity;
    [SerializeField] private float maxIntensity;
    [SerializeField] private float minDelay;
    [SerializeField] private float maxDelay;
    [SerializeField] private Color firstColor;
    [SerializeField] private Color secondColor;

    private float oldTime;
    private float delay;
    // Start is called before the first frame update
    void Start()
    {
        ChangeLight();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - oldTime >=  delay) ChangeLight();
    }

    private void ChangeLight()
    {
        float intensity = Random.Range(minIntensity, maxIntensity);
        delay = Random.Range(minDelay, maxDelay);
        Color color = Color.Lerp(firstColor, secondColor, Random.Range(0, 1));
        lightSource.intensity = intensity;
        lightSource.color = color;
        oldTime = Time.time;
    }
}
