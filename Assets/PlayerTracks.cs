using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerTracks : MonoBehaviour
{ 
    private RenderTexture splatMap;
    public Shader drawShader;

    private Material brushMaterial;
    private Material splatMaterial;

    public GameObject terrain;
    public Transform playerTransform;

    private Vector3 prevPos;
    private bool hasMoved;

    RaycastHit groundHit;
    int layerMask;

    [Range(1, 500)]
    public float brushSize;
    [Range(0, 1)]
    public float brushStrength;


    // Start is called before the first frame update
    void Start()
    {
        layerMask = LayerMask.GetMask("Ground");
        brushMaterial = new Material(drawShader);
        brushMaterial.SetVector("_Color", Color.red);

        splatMaterial = terrain.GetComponent<MeshRenderer>().material;
        splatMaterial.SetTexture("_Splat", splatMap = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGBFloat));

        prevPos.x = playerTransform.position.x;
        prevPos.y = playerTransform.position.y;
        prevPos.z = playerTransform.position.z;

        hasMoved = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (prevPos.x == playerTransform.position.x && prevPos.y == playerTransform.position.y && prevPos.z == playerTransform.position.z)
        {
            hasMoved = false;
        }
        else
        {
            hasMoved = true;
        }
        if (Physics.Raycast(playerTransform.position, Vector3.down, out groundHit, 2f, layerMask) && hasMoved)
        {
            brushMaterial.SetVector("_Coordinate", new Vector4(groundHit.textureCoord.x, groundHit.textureCoord.y, 0, 0));
            brushMaterial.SetFloat("_Strength", brushStrength);
            brushMaterial.SetFloat("_Size", brushSize);
            RenderTexture temp = RenderTexture.GetTemporary(splatMap.width, splatMap.height, 0, RenderTextureFormat.ARGBFloat);
            Graphics.Blit(splatMap, temp);
            Graphics.Blit(temp, splatMap, brushMaterial);
            RenderTexture.ReleaseTemporary(temp);
        }

        prevPos.x = playerTransform.position.x;
        prevPos.y = playerTransform.position.y;
        prevPos.z = playerTransform.position.z;
    }
}
