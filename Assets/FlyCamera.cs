using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Just a placeholder fly camera for demostrating terrain building and destruction
public class FlyCamera : MonoBehaviour
{
    public Camera cam;
    public WorldGenerator worldGen;
    [Range(1f, 5f)] public float brushSize = 3f;
    [Range(1f, 10f)] public float brushStrength = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + cam.transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal"), Time.deltaTime * 10f);
        cam.transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y"), 0, 0));
        transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0));

        if (Input.GetMouseButton(0))
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1f));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "Terrain")
                {
                    Chunk chunk = worldGen.GetChunkFromVector3(hit.transform.position);
                    if (chunk != null)
                    {
                        chunk.AddTerrain(hit.point, brushSize, brushStrength);
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
                    Chunk chunk = worldGen.GetChunkFromVector3(hit.transform.position);
                    if (chunk != null)
                    {
                        chunk.RemoveTerrain(hit.point, brushSize, brushStrength);
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
