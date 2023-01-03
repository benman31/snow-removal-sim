using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PostEffectsController : MonoBehaviour
{
    Material postEffectMat;
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        RenderTexture renderTexture = RenderTexture.GetTemporary(src.width, src.height, 0, src.format);
        
        Graphics.Blit(src, renderTexture, postEffectMat);
        Graphics.Blit(renderTexture, dest);

        RenderTexture.ReleaseTemporary(renderTexture);
    }
}
