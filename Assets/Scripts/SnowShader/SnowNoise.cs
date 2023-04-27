/*
 * Author: Benjamin Enman, 97377
 * Based on the guide by PeerPlay: https://youtu.be/LMSDFhGP73g
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is responsible for refilling snowprints
public class SnowNoise : MonoBehaviour
{
    private const float WEATHER_SNOWFALL_TO_FLAKE_AMOUNT = 0.01f;

    public Shader snowFallShader;
    private Material snowFallMaterial;

    [Range(0.001f, 0.1f)]
    public float flakeAmount;
    [Range(0, 0.005f)]
    public float flakeOpacity;
    // Start is called before the first frame update
    void Start()
    {
        snowFallMaterial = new Material(snowFallShader);
        WeatherController.OnSnowRateChange += this.HandleSnowFallRateChange;
    }

    private void OnDestroy()
    {
        WeatherController.OnSnowRateChange -= this.HandleSnowFallRateChange;
    }

    // Update is called once per frame
    void Update()
    {
        snowFallMaterial.SetFloat("_FlakeAmount", flakeAmount);
        snowFallMaterial.SetFloat("_FlakeOpacity", flakeOpacity);
        RenderTexture snow = (RenderTexture)WorldGenerator.snowMat.GetTexture("_Splat");
        RenderTexture temp = RenderTexture.GetTemporary(snow.width, snow.height, 0, RenderTextureFormat.ARGBFloat);
        Graphics.Blit(snow, temp, snowFallMaterial);
        Graphics.Blit(temp, snow);
        WorldGenerator.snowMat.SetTexture("_Splat", snow);
        RenderTexture.ReleaseTemporary(temp);
    }

    private void HandleSnowFallRateChange(float snowFallRate)
    {
        this.flakeAmount = WEATHER_SNOWFALL_TO_FLAKE_AMOUNT * Mathf.Clamp(snowFallRate, 0.0f, 10.0f);
    }
}
