using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Just a placeholder fly camera for demostrating terrain building and destruction
public class FlyCamera : MonoBehaviour
{
    public Camera cam;
    public WorldGenerator worldGen;
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
                    worldGen.GetChunkFromVector3(hit.transform.position).AddTerrain(hit.point);
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
                    worldGen.GetChunkFromVector3(hit.transform.position).RemoveTerrain(hit.point);
                }

            }
            else
            {
                Debug.Log("nothing clicked");
            }
        }
    }
}
