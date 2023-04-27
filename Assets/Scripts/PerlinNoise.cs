
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PerlinNoise : MonoBehaviour
{
    public int width = 256;
    public int height = 256;
    public float scale = 20f;
    private float xOffset = 0;
    private float yOffset = 0;

    public Material mat;

    private Texture2D texture;

    void Start()
    {
        xOffset = Random.Range(0, 9999f);
        yOffset = Random.Range(0, 9999f);
        Texture2D noiseTex = GenerateTex();
        // mat.SetTexture("_NoiseTex", noiseTex);
        // noiseTex.Apply();
    }
    public Texture2D GenerateTex()
    {
        texture = new Texture2D(width, height);
        //gen perlin noise tex
        for(int i = 0; i < width; i ++)
        {
            for(int j = 0; j < height; j++)
            {
                Color color = CalculateColor(i, j);
                texture.SetPixel(i, j, color);
            }
        }
        return texture;
    }

    Color CalculateColor(int x, int y)
    {
        float perlinX = (float)x / width;
        float perlinY = (float)y / height;

        perlinX=scale * perlinX + xOffset;
        perlinY=scale * perlinY + yOffset;
        
        float noise = Mathf.PerlinNoise(perlinX, perlinY);
        
        return new Color(noise, noise, noise);
    }

}
