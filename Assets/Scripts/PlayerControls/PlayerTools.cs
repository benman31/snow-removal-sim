using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ToolType
{
    Shovel,
    FlameThrower,
    SnowBlower,
}

public class PlayerTools : MonoBehaviour
{

    [SerializeField] private Camera cam;
    [SerializeField] private WorldGenerator worldGen;

    [Range(0f, 5f)] public float brushSize = 3f;
    [Range(0f, 50f)] public float brushStrength = 5f;

    [SerializeField] private ToolType currentTool = ToolType.Shovel;
    [SerializeField] private float shovelRange = 2f;
    [SerializeField] private Transform shovelTip;

    private AnimationStateController animController;
    private const float POISE_SCALE = 0.5f;
    private const float SNOW_LOSS_SCALE = 2.5f;

    private float playerSnowVolume = 0f;

    // Start is called before the first frame update
    void Start()
    {
        this.animController = this.GetComponentInParent<AnimationStateController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)){
            if (this.currentTool != ToolType.Shovel)
            {
                this.currentTool = ToolType.Shovel;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (this.currentTool != ToolType.FlameThrower)
            {
                this.currentTool = ToolType.FlameThrower;
            }
        }

        if (this.currentTool == ToolType.FlameThrower)
        {
            if (Input.GetMouseButton(1))
            {
                Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1f));
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.tag == "Terrain")
                    {
                        Vector3 scale = new Vector3(1f / worldGen.transform.localScale.x, 1f / worldGen.transform.localScale.y, 1f / worldGen.transform.localScale.z);
                        Chunk chunk = worldGen.GetChunkFromVector3(Vector3.Scale(hit.point, scale));
                        if (chunk != null)
                        {
                            chunk.AddTerrain(Vector3.Scale(hit.point, scale), brushSize, brushStrength);
                        }
                    }

                }
                else
                {
                    Debug.Log("nothing clicked");
                }
            }

            if (Input.GetMouseButton(0))
            {
                Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1f));
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.tag == "Terrain")
                    {
                        Vector3 scale = new Vector3(1f / worldGen.transform.localScale.x, 1f / worldGen.transform.localScale.y, 1f / worldGen.transform.localScale.z);
                        Chunk chunk = worldGen.GetChunkFromVector3(Vector3.Scale(hit.point, scale));
                        if (chunk != null)
                        {
                            chunk.RemoveTerrain(Vector3.Scale(hit.point, scale), brushSize, brushStrength);
                        }
                    }

                }
                else
                {
                    Debug.Log("nothing clicked");
                }
            }
        }

        if (this.currentTool == ToolType.Shovel)
        {
            if (animController.IsMakingHole())
            {
                Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.35f, 1f));
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, shovelRange))
                {
                    if (hit.transform.tag == "Terrain")
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
                else
                {
                    Debug.Log("nothing clicked");
                }
            }
            if (animController.IsDroppingSnow() && playerSnowVolume > 0f)
            {
                Vector3 down = shovelTip.TransformDirection(Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(shovelTip.position, down, out hit))
                {
                    if (hit.transform.tag == "Terrain")
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
}
