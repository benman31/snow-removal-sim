using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    int width
    {
        get
        {
            return GameData.ChunkWidth;
        }
    }

    int height
    {
        get
        {
            return GameData.ChunkHeight;
        }
    }

    float isoValue
    {
        get
        {
            return GameData.isoValue;
        }
    }

    public float[,,] terrainMap;
    public float[,] surfaceHeightMap;

    List<Vector3> verts = new List<Vector3>();
    List<int> triangles = new List<int>();

    public GameObject chunkObject;

    MeshFilter meshFilter;
    MeshCollider meshCollider;
    MeshRenderer meshRenderer;

    Vector3Int chunkPosition;

    public Chunk (Vector3Int pos)
    {
        chunkObject = new GameObject();
        chunkObject.name = $"Chunk {pos.x}, {pos.z}";
        chunkPosition = pos;
        chunkObject.transform.position = chunkPosition;

        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshCollider = chunkObject.AddComponent<MeshCollider>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        meshRenderer.material = Resources.Load<Material>("Materials/Terrain");

        chunkObject.transform.tag = "Terrain";

        terrainMap = new float[width + 1, height + 1, width + 1];
        surfaceHeightMap = new float[width + 1, width +1];

        populateTerrainMap();
        ClearMeshData();
        CreateMeshData2();
        BuildMesh();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void ClearMeshData()
    {
        verts.Clear();
        triangles.Clear();
    }

    void BuildMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = verts.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    void CreateMeshData2()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < width; z++)
                {

                    MarchCube2(new Vector3Int(x, y, z));
                }
            }
        }
    }

    void populateTerrainMap()
    {
        for (int x = 0; x < width + 1; x++)
        {
            for (int z = 0; z < width + 1; z++)
            {
                for (int y = 0; y < height + 1; y++)
                {
                    float surfaceHeight = GameData.GetTerrainHeight(x + chunkPosition.x, z + chunkPosition.z);

                    float surfaceValue = (float)y - surfaceHeight;
                    terrainMap[x, y, z] = surfaceValue;
                    if (surfaceValue > isoValue)
                    {
                        surfaceHeightMap[x, z] = y;
                    }
                    
                }
            }
        }
    }

    public void AddTerrain(Vector3 pos)
    {
        Vector3Int temp = new Vector3Int(Mathf.CeilToInt(pos.x), Mathf.CeilToInt(pos.y), Mathf.CeilToInt(pos.z));
        temp -= chunkPosition;

        terrainMap[temp.x, temp.y, temp.z] = 0f;
        //surfaceHeightMap[temp.x, temp.z] = temp.y;
        
        ClearMeshData();
        CreateMeshData2();
        BuildMesh();
    }

    public void RemoveTerrain(Vector3 pos)
    {
        Vector3Int temp = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
        temp -= chunkPosition;

        terrainMap[temp.x, temp.y, temp.z] = 1f;

        ClearMeshData();
        CreateMeshData2();
        BuildMesh();
    }

    float SampleTerrain(Vector3Int point)
    {
        return terrainMap[point.x, point.y, point.z];
    }

    int VertForIndex(Vector3 vert)
    {
        // Loop through all verts in vert list
        for (int i = 0; i < verts.Count; i++)
        {
            // If vert matches input then return this index
            if (verts[i] == vert)
            {
                return i;
            }
        }
        // If not in the list already then add it
        verts.Add(vert);
        return verts.Count - 1;
    }

    int GetCubeConfig(float[] cube)
    {
        int congigurationIndex = 0;
        for (int i = 0; i < 8; i++)
        {
            if (cube[i] > isoValue)
            {
                congigurationIndex |= 1 << i;
            }
        }

        return congigurationIndex;
    }

    void MarchCube2(Vector3Int coord)
    {
        // Sample terrain values at each corner of the cube

        float[] cube = new float[8];
        for (int i = 0; i < 8; i++)
        {
            cube[i] = SampleTerrain(coord + GameData.CornerTable[i]);
        }


        int configIdx = GetCubeConfig(cube);



        if (configIdx == 0 || configIdx == 255)
        {
            return;
        }

        int edgeIndex = 0;
        for (int i = 0; i < 5; i++)
        {
            for (int p = 0; p < 3; p++)
            {
                int index = GameData.triangulation[configIdx, edgeIndex];

                if (index < 0)
                {
                    return;
                }

                Vector3 vert1 = coord + GameData.CornerTable[GameData.edgeIndices[index, 0]];
                Vector3 vert2 = coord + GameData.CornerTable[GameData.edgeIndices[index, 1]];

                Vector3 vertPos;
 
                // Get the exact values of either end of the current edge
                float vert1Sample = cube[GameData.edgeIndices[index, 0]];
                float vert2Sample = cube[GameData.edgeIndices[index, 1]];

                //Calculate the difference between values
                float diff = vert2Sample - vert1Sample;
                if (diff == 0)
                {
                    diff = isoValue;
                }
                else
                {
                    diff = (isoValue - vert1Sample) / diff;
                }
                // Calculate the exact point the edge passes through
                vertPos = vert1 + ((vert2 - vert1) * diff);
                
                
                // Ensure only one of each vert is added (makes shading smooth instead of blocky) and is more efficient
                triangles.Add(VertForIndex(vertPos));

                edgeIndex++;
            }
        }

    }
}
