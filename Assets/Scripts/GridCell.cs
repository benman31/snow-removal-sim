using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    private int posX;
    private int posZ;

    private const float MAX_SNOW_VOLUME = 10f;

    // Placeholder to represent snow volume above this grid cell
    public float snowVolume;

    public bool isClear = false;

    private bool isDirty = false;

    // Start is called before the first frame update
    void Start()
    {
        this.snowVolume = MAX_SNOW_VOLUME;        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.isDirty)
        {
            print($"snow volume in cell {this.name} is {this.snowVolume}");
            float snowFraction = this.snowVolume / MAX_SNOW_VOLUME;
            Vector3 newScale = this.transform.localScale;
            float newSnowHeight = newScale.y * snowFraction;
            this.transform.localScale = new Vector3(newScale.x, newSnowHeight, newScale.z);
            this.isDirty = false;
        }
    }

    public void SetPosition(int x, int z)
    {
        posX = x;
        posZ = z;
    }

    public Vector2 GetPosition()
    {
        return new Vector2(posX, posZ);
    }

    public void SetSnowVolume(float volume)
    {
        this.snowVolume = System.Math.Clamp(volume, 0, MAX_SNOW_VOLUME);
        this.isDirty = true;
    }

    public float GetSnowVolume()
    {
        return this.snowVolume;
    }

    public void ShovelSnow()
    {
        print($"Shoveled snow before: {this.snowVolume}");
        this.snowVolume = System.Math.Max(this.snowVolume - 1f, 0);
        print($"Shoveled snow after: {this.snowVolume}");
        this.isDirty = true;
    }

    public void AddSnow()
    {
        this.snowVolume = System.Math.Min(this.snowVolume + 1, MAX_SNOW_VOLUME);
        this.isDirty = true;
    }
}
