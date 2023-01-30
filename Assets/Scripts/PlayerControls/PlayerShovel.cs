using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShovel : MonoBehaviour
{

    public Camera cam;
    public WorldGenerator worldGen;
    [Range(0f, 20f)] public float brushSize = 3f;
    [Range(1f, 50f)] public float brushStrength = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
                        chunk.AddTerrain(Vector3.Scale(hit.point, scale), brushSize / worldGen.scale, brushStrength);
                    }
                }

            }
            else
            {
                Debug.Log("nothing clicked");
            }
        }

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
                        chunk.RemoveTerrain(Vector3.Scale(hit.point, scale), brushSize / worldGen.scale, brushStrength);
                    }
                }

            }
            else
            {
                Debug.Log("nothing clicked");
            }
        }
    }
}
