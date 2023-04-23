/*
Written by: Abdelrahman Awad
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PostProcessingCamera : MonoBehaviour
{
    RenderTexture mainRenderTexture;
    [SerializeField] Material postProcessMat;
    [SerializeField] Material postProcessMat2;

    //mat1 
    [HideInInspector] public float size = 4;
    [HideInInspector] public float dropletsSpeed = 1;
    private float currentSize = 1;
    private float currentDropletsSpeed = 1;
    private float currentDistortion = 1;
    [HideInInspector] public float distortion = 1;

    //mat2
    [HideInInspector] public float radius = 1.5f;
    [HideInInspector] public float feather = 0.5f;
    [HideInInspector] public float Intensity = 0;
    [HideInInspector] public int waterShaderPass = 0;

    private float currentFeather = 0.5f;
    private float currentRadius = 1.5f;
    private float currentIntensity = 0;

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        //adjust post effect parameters before passing them to shader
        if (currentSize != size)
        {
            postProcessMat.SetFloat("_Size", size);
            currentSize = size;
        }

        if (currentDropletsSpeed != dropletsSpeed)
        {
            postProcessMat.SetFloat("_TimeScale", dropletsSpeed);
            currentDropletsSpeed = dropletsSpeed;
        }

        if (currentDistortion != distortion)
        {
            postProcessMat.SetFloat("_Distortion", distortion);
            currentDistortion = distortion;
        }

        if (currentRadius != radius)
        {
            postProcessMat2.SetFloat("_Radius", radius);
            currentRadius = radius;
        }

        if (currentFeather != feather)
        {
            postProcessMat2.SetFloat("_Feather", feather);
            currentFeather = feather;
        }

        if (currentIntensity != Intensity)
        {
            postProcessMat2.SetFloat("_Intensity", Intensity);
            currentIntensity = Intensity;
        }

        RenderTexture renderTexture = RenderTexture.GetTemporary(src.width, src.height, 0, src.format);
        RenderTexture renderTexture2 = RenderTexture.GetTemporary(src.width, src.height, 0, src.format);

        Graphics.Blit(src, renderTexture, postProcessMat2, 0);
        Graphics.Blit(renderTexture, renderTexture2, postProcessMat, waterShaderPass);
        Graphics.Blit(renderTexture2, dest);

        RenderTexture.ReleaseTemporary(renderTexture);
        RenderTexture.ReleaseTemporary(renderTexture2);
    }
}
