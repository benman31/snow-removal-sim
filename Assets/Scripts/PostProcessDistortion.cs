/*
Written by: Abdelrahman Awad
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PostProcessDistortion : MonoBehaviour
{
    public Shader distortionShader;
    public Camera camera;
    [Range(0, 1)] public float distortionStr = .1f;
    [SerializeField] private Material postProcessMat;

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        RenderTexture renderTexture = RenderTexture.GetTemporary(src.width, src.height, 0, src.format);
        RenderTexture disortionRenderTex = RenderTexture.GetTemporary(src.width, src.height, 0, src.format);

        if (camera == null) //check if camera is unassigned
        {}
        else
        {
            camera.targetTexture = disortionRenderTex;
            camera.Render();
        } 

        postProcessMat.SetTexture("_DistortionTexture", disortionRenderTex);
        postProcessMat.SetFloat("_DistortionStr", distortionStr);

        Graphics.Blit(src, renderTexture, postProcessMat, 0);
        Graphics.Blit(renderTexture, dest);

        RenderTexture.ReleaseTemporary(renderTexture);
        RenderTexture.ReleaseTemporary(disortionRenderTex);
    }
}
