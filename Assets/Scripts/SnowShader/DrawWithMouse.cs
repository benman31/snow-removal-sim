/*
 * Author: Benjamin Enman, 97377
 * Based on the guide by PeerPlay: https://youtu.be/_NfxMMzYwgo
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawWithMouse : MonoBehaviour
{
    public Camera mainCamera;
    public Shader drawShader;
    public Shader fillShader;

    private RenderTexture splatmap;
    private Material fillMaterial, drawMaterial;

    private MeshRenderer meshRenderer;
    
    private RaycastHit hit;

    [Range(1, 500)]
    public float brushSize;
    [Range(0, 1)]
    public float brushStrength;

    // Start is called before the first frame update
    void Start()
    {
        drawMaterial = new Material(drawShader);
        fillMaterial = new Material(fillShader);
        meshRenderer = GetComponent<MeshRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit))
            {
                drawMaterial.SetVector("_Coordinate", new Vector4(hit.textureCoord.x, hit.textureCoord.y, 0, 0));
                drawMaterial.SetFloat("_Strength", brushStrength);
                drawMaterial.SetFloat("_Size", brushSize);

                RenderTexture snow = (RenderTexture)meshRenderer.material.GetTexture("_Splat");
                RenderTexture temp = RenderTexture.GetTemporary(snow.width, snow.height, 0, RenderTextureFormat.ARGBFloat);
                Graphics.Blit(snow, temp, drawMaterial);
                Graphics.Blit(temp, snow);
                meshRenderer.material.SetTexture("_Splat", snow);
                RenderTexture.ReleaseTemporary(temp);
            }
        }
        else if (Input.GetKey(KeyCode.Mouse1))
        {
            if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit))
            {
                fillMaterial.SetVector("_Coordinate", new Vector4(hit.textureCoord.x, hit.textureCoord.y, 0, 0));
                fillMaterial.SetFloat("_Strength", brushStrength);
                fillMaterial.SetFloat("_Size", brushSize);

                RenderTexture snow = (RenderTexture)meshRenderer.material.GetTexture("_Splat");
                RenderTexture temp = RenderTexture.GetTemporary(snow.width, snow.height, 0, RenderTextureFormat.ARGBFloat);
                Graphics.Blit(snow, temp, fillMaterial);
                Graphics.Blit(temp, snow);
                meshRenderer.material.SetTexture("_Splat", snow);
                RenderTexture.ReleaseTemporary(temp);
            }
        }

    }

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, 256, 256), splatmap, ScaleMode.ScaleToFit, false, 1);
    }
}
