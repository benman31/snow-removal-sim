using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PostProcessingCamera : MonoBehaviour
{
    public float size = 1;
    public float timeScale = 1;

    private float currentSize = 1;
    private float currentTimeScale = 1;
    private float currentDistortion = 1;
    [Range(-5, 5)] public float distortion = 1;
    [SerializeField] Material postProcessMat;

    private void OnPreRender()
    {
        if(currentSize != size)
        {
            postProcessMat.SetFloat("_Size", size);
            currentSize = size;
        }
        
        if(currentTimeScale != timeScale)
        {
            postProcessMat.SetFloat("_TimeScale", timeScale);
            currentTimeScale = timeScale;
        }

        if(currentDistortion != distortion)
        {
            postProcessMat.SetFloat("_Distortion", distortion);
            currentDistortion = distortion;
        }
    }
    private void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        Graphics.Blit(source, null as RenderTexture, postProcessMat);
    }
}
