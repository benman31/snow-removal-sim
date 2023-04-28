//Author: Benjamin Enman

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public enum ToolType
{
    Shovel,
    FlameThrower,
    SnowBlower,
}

public class PlayerTools : MonoBehaviour
{
    public static event Action<bool> OnShovelActive;
    public static event Action<bool> OnSnowblowerActive;
    public static event Action<bool> OnFlamethrowerActive;

    [SerializeField] private Camera cam;
    [SerializeField] private WorldGenerator worldGen;

    [Range(0f, 5f)] public float brushSize = 3f;
    [Range(0f, 50f)] public float brushStrength = 5f;

    [SerializeField] private ToolType currentTool = ToolType.Shovel;
    [SerializeField] private float shovelRange = 10f;
    [SerializeField] private Transform shovelTip;
    [SerializeField] private Transform handsPointOfTransform;


    private AnimationStateController animController;
    private const float POISE_SCALE = 1.5f;
    private const float SNOW_LOSS_SCALE = 0.25f;

    private float playerSnowVolume = 0f;

    // Start is called before the first frame update
    void Start()
    {
        this.animController = this.GetComponentInParent<AnimationStateController>();
        SnowBlowerController.OnSnowCollision += this.HandleSnowBlower;
        SnowblowerTrajectory.OnGroundCollision += this.HandleSnowBlowerTrajectory;
        Dig.OnSnowCollision += this.HandleDig;
    }

    private void OnDestroy()
    {
        SnowBlowerController.OnSnowCollision -= this.HandleSnowBlower;
        SnowblowerTrajectory.OnGroundCollision -= this.HandleSnowBlowerTrajectory;
        Dig.OnSnowCollision -= this.HandleDig;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log($"Current tool: {this.currentTool}");
        if (Input.GetKeyDown(KeyCode.Alpha1)){
            if (this.currentTool != ToolType.Shovel)
            {
                this.currentTool = ToolType.Shovel;
                OnShovelActive?.Invoke(true);
                OnSnowblowerActive?.Invoke(false);
                OnFlamethrowerActive?.Invoke(false);

                handsPointOfTransform?.gameObject.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (this.currentTool != ToolType.SnowBlower && !animController.IsPlayerDigging() && !animController.IsCarryingSnow())
            {
                Debug.Log($"Switching tools to SNOWBLOWER: {OnSnowblowerActive.GetInvocationList().Length}");
                this.currentTool = ToolType.SnowBlower;
                OnShovelActive?.Invoke(false);
                OnSnowblowerActive?.Invoke(true);
                OnFlamethrowerActive?.Invoke(false);

                // handsPointOfTransform?.gameObject.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && !animController.IsPlayerDigging() && !animController.IsCarryingSnow())
        {
            if (this.currentTool != ToolType.FlameThrower)
            {
                this.currentTool = ToolType.FlameThrower;
                OnShovelActive?.Invoke(false);
                OnSnowblowerActive?.Invoke(false);
                OnFlamethrowerActive?.Invoke(true);

                handsPointOfTransform?.gameObject.SetActive(true);
            }
        }

        if (this.currentTool == ToolType.FlameThrower)
        {
            if (Input.GetMouseButton(0))
            {
                Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 2f));
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.tag == "Terrain" || hit.transform.tag == "Snow")
                    {
                        Vector3 scale = new Vector3(1f / worldGen.transform.localScale.x, 1f / worldGen.transform.localScale.y, 1f / worldGen.transform.localScale.z);
                        Chunk chunk = worldGen.GetChunkFromVector3(Vector3.Scale(hit.point, scale));
                        if (chunk != null)
                        {
                            chunk.RemoveTerrain(Vector3.Scale(hit.point, scale), brushSize, brushStrength);
                        }
                    }

                }
            }
        }

        if (this.currentTool == ToolType.Shovel)
        {
            /*if (animController.IsPlayerDigging() || animController.IsMakingHole() || animController.IsCarryingSnow())
            {
                Ray ray = cam.ViewportPointToRay(new Vector3(0.0f, 0f, 0f));
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, shovelRange))
                {
                    if (hit.transform.tag == "Terrain" || hit.transform.tag == "Snow")
                    {
                        Vector3 scale = new Vector3(1f / worldGen.transform.localScale.x, 1f / worldGen.transform.localScale.y, 1f / worldGen.transform.localScale.z);
                        Chunk chunk = worldGen.GetChunkFromVector3(Vector3.Scale(hit.point, scale));
                        if (chunk != null)
                        {
                            chunk.RemoveTerrain(Vector3.Scale(hit.point, scale), brushSize, animController.poise * POISE_SCALE);
                            playerSnowVolume += 1f;
                        }
                    }

                }
            }*/
            if (animController.IsDroppingSnow() && playerSnowVolume > 0f)
            {
                Vector3 down = shovelTip.TransformDirection(Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(shovelTip.position, down, out hit))
                {
                    if (hit.transform.tag == "Terrain" || hit.transform.tag == "Snow")
                    {
                        Vector3 scale = new Vector3(1f / worldGen.transform.localScale.x, 1f / worldGen.transform.localScale.y, 1f / worldGen.transform.localScale.z);
                        Chunk chunk = worldGen.GetChunkFromVector3(Vector3.Scale(hit.point, scale));
                        if (chunk != null)
                        {
                            
                            chunk.AddTerrain(Vector3.Scale(hit.point, scale), brushSize, playerSnowVolume);
                            playerSnowVolume = Mathf.Max(playerSnowVolume - 1 * SNOW_LOSS_SCALE, 0f);
                            
                        }
                    }

                }
            }
        }
    }

    // Temp experiment
    public void HandleDig(Vector3 point)
    {
        if (this.currentTool == ToolType.Shovel)
        {
            Vector3 scale = new Vector3(1f / worldGen.transform.localScale.x, 1f / worldGen.transform.localScale.y, 1f / worldGen.transform.localScale.z);
            Chunk chunk = worldGen.GetChunkFromVector3(Vector3.Scale(point, scale));
            if (chunk != null)
            {
                // TODO make these hard coded values into const, or editor props
                //chunk.RemoveTerrain(Vector3.Scale(point, scale), 2.5f, 50);
                chunk.RemoveTerrain(Vector3.Scale(point, scale), brushSize, animController.poise * POISE_SCALE);
                playerSnowVolume += 1f;
            }
        }
    }

    public void HandleSnowBlower(Vector3 point)
    {
        if (this.currentTool == ToolType.SnowBlower)
        {   
            Vector3 scale = new Vector3(1f / worldGen.transform.localScale.x, 1f / worldGen.transform.localScale.y, 1f / worldGen.transform.localScale.z);
            Chunk chunk = worldGen.GetChunkFromVector3(Vector3.Scale(point, scale));
            if (chunk != null)
            {
                // TODO make these hard coded values into const, or editor props
                chunk.RemoveTerrain(Vector3.Scale(point, scale), 2.5f, 50);
            }
        }
    }

    public void HandleSnowBlowerTrajectory(Vector3 point)
    {
        if (this.currentTool == ToolType.SnowBlower)
        {
            Vector3 scale = new Vector3(1f / worldGen.transform.localScale.x, 1f / worldGen.transform.localScale.y, 1f / worldGen.transform.localScale.z);
            Chunk chunk = worldGen.GetChunkFromVector3(Vector3.Scale(point, scale));
            if (chunk != null)
            {
                // TODO make these hard coded values into const, or editor props
                // TODO make specialized "snow pile" function for more realistic behavior
                chunk.AddTerrain(Vector3.Scale(point, scale), 8f, 2f);
            }
        }
    }
}
