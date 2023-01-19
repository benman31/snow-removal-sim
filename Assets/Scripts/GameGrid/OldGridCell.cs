/*
 * Author: Benjamin Enman, 97377
 * Based on the guide by MetalStorm Games: https://www.youtube.com/watch?v=qkSSdqOAAl4
 */
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OldGridCell : MonoBehaviour
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
        // No need to update if completely clear
        if (this.isClear)
        {
            return;
        }

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
    // Set position in grid coordinates
    public void SetPosition(int x, int z)
    {
        posX = x;
        posZ = z;
    }
    // Get position in grid
    public Vector2Int GetPosition()
    {
        return new Vector2Int(posX, posZ);
    }

    // Set the scale of the local transform (for debug visuals)
    public void SetScale(float x, float y, float z)
    {
        this.transform.localScale = new Vector3(x, y, z);
    }

    // Scale by a single scalar (for debug visuals)
    public void Scale(float scalar)
    {
        this.transform.localScale *= scalar;
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

        if (this.snowVolume <= 0)
        {
            this.isClear = true;
        }
    }

    public void AddSnow()
    {
        this.snowVolume = System.Math.Min(this.snowVolume + 1, MAX_SNOW_VOLUME);
        this.isDirty = true;
    }

    private Vector2[] getCellsAroundTarget()
    {
        return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Targetting"))
        {
            this.GetComponentInChildren<MeshRenderer>().material.color = Color.green;
            this.GetComponentInParent<GridInputManager>().AddHighlightedCell(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Targetting"))
        {
            this.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
            this.GetComponentInParent<GridInputManager>().RemoveHighlightedCell(this);
        }
    }
}