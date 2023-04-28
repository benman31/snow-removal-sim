/*
Written by: Abdelrahman Awad
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlayDissolveEffect : MonoBehaviour
{
    public GameObject[] gameObjects;
    public PerlinNoise noise;
    public Color tint;
    public Vector4 edgeColor;
    private bool playEffect = false;
    private bool playEffectReverse = true;
    private Renderer[] renderers;
    private Material[] mats;
    private Texture2D noiseTex;
    private float dissolveAmount;
    private float edgeThickness;
    private float speed = 2f;
    private float dissolveAmountReverse;
    private float edgeThicknessReverse;

    void Awake()
    {
        renderers = new Renderer[gameObjects.Length];
        mats = new Material[gameObjects.Length];

        for (int i = 0; i < gameObjects.Length; i++)
        {
            renderers[i] = gameObjects[i].GetComponent<Renderer>();
            //get mat, apply noise tex and init vars
            mats[i] = new Material(renderers[i].sharedMaterial); //create mat instance
            renderers[i].material = mats[i];
            noiseTex = noise.GenerateTex();
            mats[i].SetTexture("_NoiseTex", noiseTex);
            noiseTex.Apply();

            dissolveAmountReverse = 1f;
            edgeThicknessReverse = .5f;
            dissolveAmount = 0;
            edgeThickness = 0;
            mats[i].SetColor("_Tint", tint);
            mats[i].SetVector("_EdgeColor", edgeColor);
            mats[i].SetFloat("_DissolveAmount", dissolveAmount);
            mats[i].SetFloat("_EdgeThickness", edgeThickness);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < gameObjects.Length; i++)
        {
            if (playEffect)
            {
                mats[i].SetFloat("_DissolveAmount", dissolveAmount);
                mats[i].SetFloat("_EdgeThickness", edgeThickness);
                dissolveAmount += (Time.deltaTime * speed) / gameObjects.Length;
                edgeThickness += (Time.deltaTime * speed/2) / gameObjects.Length;

                if (dissolveAmount > 1.1f)
                {
                    dissolveAmount = 0;
                    edgeThickness = 0;
                    playEffect = false;
                }
            }
            else if (playEffectReverse)
            {
                mats[i].SetFloat("_DissolveAmount", dissolveAmountReverse);
                mats[i].SetFloat("_EdgeThickness", edgeThicknessReverse);
                dissolveAmountReverse -= (Time.deltaTime * speed) / gameObjects.Length;
                edgeThicknessReverse -= (Time.deltaTime * speed/2) / gameObjects.Length;

                if (dissolveAmountReverse < -.1f)
                {
                    dissolveAmountReverse = 1f;
                    edgeThicknessReverse = .5f;
                    playEffectReverse = false;
                }
            }
            mats[i].SetColor("_Tint", tint);
            mats[i].SetVector("_EdgeColor", edgeColor);
        }
    }

    public void Play()
    {
        playEffect = true;
    }

    public void PlayReverse()
    {
        playEffectReverse = true;
    }
}
