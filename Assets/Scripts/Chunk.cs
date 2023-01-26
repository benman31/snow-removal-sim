using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Animations;
using static UnityEditor.PlayerSettings;

public class Chunk
{
    WorldGenerator world;

    int width
    {
        get
        {
            return world.ChunkWidth;
        }
    }

    int height
    {
        get
        {
            return world.ChunkHeight;
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
    public int [,] surfaceHeightMap;

    List<Vector3> verts = new List<Vector3>();
    List<int> triangles = new List<int>();

    public GameObject chunkObject;

    MeshFilter meshFilter;
    MeshCollider meshCollider;
    MeshRenderer meshRenderer;

    private Vector3Int chunkPosition;

    public Dictionary<Vector3Int, Chunk> neighboringChunks = new Dictionary<Vector3Int, Chunk>();
    public Boolean isDirty = false;

    public Chunk (Vector3Int pos, WorldGenerator parent)
    {
        chunkObject = new GameObject();
        chunkObject.name = $"Chunk {pos.x}, {pos.z}";
        chunkPosition = pos;
        chunkObject.transform.position = chunkPosition;
        chunkObject.layer = LayerMask.NameToLayer("Ground");

        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshCollider = chunkObject.AddComponent<MeshCollider>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        meshRenderer.material = Resources.Load<Material>("Materials/Terrain");
        //meshRenderer.material = Resources.Load<Material>("Materials/TerrainSplat");

        chunkObject.transform.tag = "Terrain";

        chunkObject.transform.SetParent(parent.transform);

        world = parent;

        terrainMap = new float[width + 1, height + 1, width + 1];
        surfaceHeightMap = new int[width + 1, width +1];

        populateTerrainMap();
        ClearMeshData();
        CreateMeshData2();
        BuildMesh();
    }

    // Update is called once per frame
    public void Update()
    {
        if (this.isDirty)
        {
            ClearMeshData();
            CreateMeshData2();
            BuildMesh();
            this.isDirty = false;
        }
        
    }

    public Vector3Int getChunkPosition()
    {
        return chunkPosition;
    }

    public Vector3Int worldPointPositiontoGridPosition(Vector3 pos)
    {
        Vector3Int localPos = new Vector3Int(Mathf.CeilToInt(pos.x), Mathf.CeilToInt(pos.y), Mathf.CeilToInt(pos.z));
        localPos -= chunkPosition;
        return localPos;
    }

    public float GetCubeVolume(Vector3Int pos)
    {
        if (pos.x > width - 1 || pos.y > height - 1 || pos.z > width - 1)
        {
            Debug.Log("Cell position out of bounds");
            return -1f;
        }
        
        float volume = 0f;

        for (int i = 0; i < 8; i++)
        {
            Vector3Int cornerCoord = new Vector3Int(pos.x, pos.y, pos.z) + GameData.CornerTable[i];
            float surfaceVal = terrainMap[cornerCoord.x, cornerCoord.y, cornerCoord.z];
            if (surfaceVal <= isoValue)
            {
                //volume += (1f / 8f);
                // For now I'll treat each cube as a binary 'all snow' or 'no snow' to avoid any tricky errors
                volume = 1f;
                break;
            }
        }

        return volume;
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

    private float GetTerrainHeight(int x, int z)
    {
        return world.TerrainHeightRange * Mathf.PerlinNoise((float)x / 16f + 1.5f + 0.001f, (float)z / 16f * 1.5f + 0.001f) + world.BaseTerrainHeight;
    }

    void populateTerrainMap()
    {
        for (int x = 0; x < width + 1; x++)
        {
            for (int z = 0; z < width + 1; z++)
            {
                int count = 0;
                for (int y = 0; y < height + 1; y++)
                {
                    float surfaceHeight = this.GetTerrainHeight(x + chunkPosition.x, z + chunkPosition.z);
                    
                    float surfaceValue = (float)y - surfaceHeight;
                    terrainMap[x, y, z] = surfaceValue;
                    
                    if (y > 0)
                    {
                        float prevSurfaceValue = surfaceValue - 1f;
                        if (surfaceValue > isoValue && prevSurfaceValue <= isoValue)
                        {
                            surfaceHeightMap[x, z] = y;
                            count++;
                            if (count > 1)
                            {
                                Debug.Log("Something went wrong, we have 2 surface borders");
                            }
                        }
                    }
                    else
                    {
                        if (surfaceValue > isoValue) {
                            surfaceHeightMap[x, z] = y;
                        }
                    }
                    
                }
            }
        }
    }

    public void UpdateTerrainAtPosition(Vector3Int pos, float value)
    {
        if (pos.x > width || pos.x < 0 || pos.y > height || pos.y < 0 || pos.z > width || pos.z < 0)
        {
            Debug.Log("Attempted to update terrain positon outside bounds of terrainMap");
            return;
        }

        if (surfaceHeightMap[pos.x, pos.z] == pos.y)
        {
            // Adding terrain to highest surface point, update heightmap

            float oldSurfaceValue = this.terrainMap[pos.x, pos.y, pos.z];
            float newSurfaceValue = oldSurfaceValue + value;

            if (newSurfaceValue < oldSurfaceValue)
            {
                if (pos.y < this.height && newSurfaceValue <= isoValue && terrainMap[pos.x, pos.y + 1, pos.z] > isoValue)
                {
                    surfaceHeightMap[pos.x, pos.z]++;
                }
            }
            // Removing terrain from heighest surface point, update heightmap
            else if (newSurfaceValue > oldSurfaceValue)
            {
                // TODO: fix this so that it loops until finding the next highest surface
                if (pos.y > 0 && newSurfaceValue > isoValue)
                {
                    /*while (terrainMap[pos.x, surfaceHeightMap[pos.x, pos.z], pos.z] > isoValue)
                    {
                        surfaceHeightMap[pos.x, pos.z]--;
                    }*/ 
                }
            }
        }
        this.terrainMap[pos.x, pos.y, pos.z] += value;
        
        if (!this.isDirty)
        {
            this.isDirty = true;
            world.gameGrid.AddChunkToDirtyList(this);
        }
    }

    public void AddTerrain(Vector3 pos, float radius, float strength)
    {
        Vector3Int localPos = worldPointPositiontoGridPosition(pos);
        UpdatePointsWithinRadius(localPos, radius, -strength);
    }

    public void RemoveTerrain(Vector3 pos, float radius, float strength)
    {
        Vector3Int localPos = worldPointPositiontoGridPosition(pos);
        UpdatePointsWithinRadius(localPos, radius, strength);
    }

    private void UpdatePointsWithinRadius(Vector3 pos, float radius, float value)
    {

        float xStart = pos.x - radius;
        float yStart = Mathf.Max(pos.y - radius, 0f);
        float zStart = pos.z - radius;

        float xEnd = pos.x + radius;
        float yEnd = Mathf.Min(pos.y + radius, height);
        float zEnd = pos.z + radius;

        float sqrRad = radius * radius;

        Vector3 chunkOffset = new Vector3(0f, 0f, 0f);


        for (float x = xStart; x < xEnd; x++)
        {
            for (float y = yStart; y < yEnd; y++)
            {
                for (float z = zStart; z < zEnd; z++)
                {

                    Chunk chunk;
                    if (x < 0 && z < 0)
                    {
                        if (!GetUpperLeftNeighbor(out chunk))
                        {
                            continue;
                        };
                    }
                    else if (x >= 0 && x <= width && z < 0)
                    {
                        if (!GetUpperNeighbor(out chunk))
                        {
                            continue;
                        }

                    }
                    else if (x > width && z < 0)
                    {
                        if (!GetUpperRightNeighbor(out chunk))
                        {
                            continue;
                        }
                    }
                    else if (x > width && z >= 0 && z <= width)
                    {
                        if (!GetRightNeighbor(out chunk))
                        {
                            continue;
                        }
                    }
                    else if (x > width && z > width)
                    {
                        if (!GetLowerRightNeighbor(out chunk)){
                            continue;
                        }
                    }
                    else if (x >= 0 && x <= width && z > width)
                    {
                        if (!GetLowerNeighbor(out chunk)){
                            continue;
                        }
                    }
                    else if (x < 0 && z > width)
                    {
                        if (!GetLowerLeftNeighbor(out chunk))
                        {
                            continue;
                        }
                    }
                    else if (x < 0 && z >= 0 && z <= width)
                    {
                        if(!GetLeftNeighbor(out chunk))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        chunk = this;
                    }

                    chunkOffset.x = x;
                    chunkOffset.y = 0f;
                    chunkOffset.z = z;

                    chunkOffset += this.chunkPosition;
                    chunkOffset -= chunk.chunkPosition;

                    float sqrDist = Mathf.Min(Vector3.SqrMagnitude(new Vector3(x, y, z) - pos), sqrRad);
                    float weight = (1f - sqrDist / sqrRad) * Time.deltaTime;
                    chunk.UpdateTerrainAtPosition(new Vector3Int((int)chunkOffset.x, (int)y, (int)chunkOffset.z), weight * value);
                    chunk.UpdateSharedPoints(new Vector3Int((int)chunkOffset.x, (int)y, (int)chunkOffset.z), weight * value);
                }
            }
        }
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

    public void GrowOverTime()
    {
        for (int x = 0; x < this.width + 1; x++)
        {
            for (int z = 0; z < this.width + 1; z++)
            {
                int surfaceHeightIdx = surfaceHeightMap[x, z];
                if (surfaceHeightIdx >= this.height)
                {
                    Debug.Log("Height map error");
                }
                float rand = UnityEngine.Random.Range(0f, 1f);

                Vector3Int pos = new Vector3Int(x, surfaceHeightIdx, z);
                if (pos.x == 0 && pos.z == 0)
                {
                    rand /= 4;
                }
                else if (pos.x == 0 && pos.z == width)
                {
                    rand /= 4;
                }
                else if (pos.x == width && pos.z == 0)
                {
                    rand /= 4;
                }
                else if (pos.x == width && pos.z == width)
                {
                    rand /= 4;
                }
                // Left edge, we share this point with 1 neighboring chunk
                else if (pos.x == 0)
                {
                    rand /= 2;
                }
                // Right edge, we share this point with 1 neighboring chunk
                else if (pos.x == width)
                {
                    rand /= 2;
                }
                // Upper edge, we share this point with one neighbor
                else if (pos.z == 0)
                {
                    rand /= 2;

                }
                // Lower edge, we share this point with 1 neighboring chunk
                else if (pos.z == width)
                {
                    rand /= 2;

                }
                UpdateTerrainAtPosition(pos, -rand);
                UpdateSharedPoints(pos, -rand);
            }
        }
    }

    private void UpdateSharedPoints(Vector3Int pos, float value)
    {   
        // Corner Cases, we have 3 neighboring chunks sharing this point
        if (pos.x == 0 && pos.z == 0)
        {
            Chunk neighbor;
            // Get left chunk and update at upper right corner
            if (this.neighboringChunks.TryGetValue(new Vector3Int(this.chunkPosition.x - width, this.chunkPosition.y, this.chunkPosition.z), out neighbor))
            {
                Vector3Int neighborPoint = new Vector3Int(width, pos.y, 0);
                neighbor.UpdateTerrainAtPosition(neighborPoint, value);
                //UpdateSharedPointHelper(neighbor, neighborPoint, pos, value);
            }

            // Get upper left chunk and update at bottom right corner
            if (this.neighboringChunks.TryGetValue(new Vector3Int(this.chunkPosition.x - width, this.chunkPosition.y, this.chunkPosition.z - width), out neighbor))
            {
                Vector3Int neighborPoint = new Vector3Int(width, pos.y, width);
                neighbor.UpdateTerrainAtPosition(neighborPoint, value);
                //UpdateSharedPointHelper(neighbor, neighborPoint, pos, value);
            }

            // Get upper chunk and update at bottom left corner
            if (this.neighboringChunks.TryGetValue(new Vector3Int(this.chunkPosition.x, this.chunkPosition.y, this.chunkPosition.z - width), out neighbor))
            {
                Vector3Int neighborPoint = new Vector3Int(0, pos.y, width);
                neighbor.UpdateTerrainAtPosition(neighborPoint, value);
                //UpdateSharedPointHelper(neighbor, neighborPoint, pos, value);
            }
        }
        else if (pos.x == 0 && pos.z == width)
        {
            Chunk neighbor;
            // Get left chunk and update at lower right corner
            if (this.neighboringChunks.TryGetValue(new Vector3Int(this.chunkPosition.x - width, this.chunkPosition.y, this.chunkPosition.z), out neighbor))
            {
                Vector3Int neighborPoint = new Vector3Int(width, pos.y, width);
                neighbor.UpdateTerrainAtPosition(neighborPoint, value);
                //UpdateSharedPointHelper(neighbor, neighborPoint, pos, value);
            }

            // Get lower left chunk and update at upper right corner
            if (this.neighboringChunks.TryGetValue(new Vector3Int(this.chunkPosition.x - width, this.chunkPosition.y, this.chunkPosition.z + width), out neighbor))
            {
                Vector3Int neighborPoint = new Vector3Int(width, pos.y, 0);
                neighbor.UpdateTerrainAtPosition(neighborPoint, value);
                //UpdateSharedPointHelper(neighbor, neighborPoint, pos, value);
            }

            // Get lower chunk and update at upper left corner
            if (this.neighboringChunks.TryGetValue(new Vector3Int(this.chunkPosition.x, this.chunkPosition.y, this.chunkPosition.z + width), out neighbor))
            {
                Vector3Int neighborPoint = new Vector3Int(0, pos.y, 0);
                neighbor.UpdateTerrainAtPosition(neighborPoint, value);
                //UpdateSharedPointHelper(neighbor, neighborPoint, pos, value);
            }
        }
        else if (pos.x == width && pos.z == 0)
        {
            Chunk neighbor;
            // Get upper chunk and update at lower right corner
            if (this.neighboringChunks.TryGetValue(new Vector3Int(this.chunkPosition.x, this.chunkPosition.y, this.chunkPosition.z - width), out neighbor))
            {
                Vector3Int neighborPoint = new Vector3Int(width, pos.y, width);
                neighbor.UpdateTerrainAtPosition(neighborPoint, value);
                //UpdateSharedPointHelper(neighbor, neighborPoint, pos, value);
            }
            // Get upper right chunk and update at lower left corner
            if (this.neighboringChunks.TryGetValue(new Vector3Int(this.chunkPosition.x + width, this.chunkPosition.y, this.chunkPosition.z - width), out neighbor))
            {
                Vector3Int neighborPoint = new Vector3Int(0, pos.y, width);
                neighbor.UpdateTerrainAtPosition(neighborPoint, value);
                //UpdateSharedPointHelper(neighbor, neighborPoint, pos, value);
            }

            // Get right chunk and update at upper left corner
            if (this.neighboringChunks.TryGetValue(new Vector3Int(this.chunkPosition.x + width, this.chunkPosition.y, this.chunkPosition.z), out neighbor))
            {
                Vector3Int neighborPoint = new Vector3Int(0, pos.y, 0);
                neighbor.UpdateTerrainAtPosition(neighborPoint, value);
                //UpdateSharedPointHelper(neighbor, neighborPoint, pos, value);
            }
        }
        else if (pos.x == width && pos.z == width)
        {
            Chunk neighbor;
            // Get right chunk and update at lower left corner
            if (this.neighboringChunks.TryGetValue(new Vector3Int(this.chunkPosition.x + width, this.chunkPosition.y, this.chunkPosition.z), out neighbor))
            {
                Vector3Int neighborPoint = new Vector3Int(0, pos.y, width);
                neighbor.UpdateTerrainAtPosition(neighborPoint, value);
                //UpdateSharedPointHelper(neighbor, neighborPoint, pos, value);
            }
            // Get lower right chunk and update at upper left corner
            if (this.neighboringChunks.TryGetValue(new Vector3Int(this.chunkPosition.x + width, this.chunkPosition.y, this.chunkPosition.z + width), out neighbor))
            {
                Vector3Int neighborPoint = new Vector3Int(0, pos.y, 0);
                neighbor.UpdateTerrainAtPosition(neighborPoint, value);
                //UpdateSharedPointHelper(neighbor, neighborPoint, pos, value);
            }
            // Get lower chunk and update at upper right corner
            if (this.neighboringChunks.TryGetValue(new Vector3Int(this.chunkPosition.x, this.chunkPosition.y, this.chunkPosition.z + width), out neighbor))
            {
                Vector3Int neighborPoint = new Vector3Int(width, pos.y, 0);
                neighbor.UpdateTerrainAtPosition(neighborPoint, value);
                //UpdateSharedPointHelper(neighbor, neighborPoint, pos, value);
            }
        }
        // Left edge, we share this point with 1 neighboring chunk
        else if (pos.x == 0)
        {
            Chunk neighbor;
            // Get left neighbor and update poiint at right edge
            if (this.neighboringChunks.TryGetValue(new Vector3Int(this.chunkPosition.x - width, this.chunkPosition.y, this.chunkPosition.z), out neighbor))
            {
                Vector3Int neighborPoint = new Vector3Int(width, pos.y, pos.z);
                neighbor.UpdateTerrainAtPosition(neighborPoint, value);
                //UpdateSharedPointHelper(neighbor, neighborPoint, pos, value);
            }
        }
        // Right edge, we share this point with 1 neighboring chunk
        else if (pos.x == width)
        {
            Chunk neighbor;
            // Get right neighbor and update point at left edge
            if (this.neighboringChunks.TryGetValue(new Vector3Int(this.chunkPosition.x + width, this.chunkPosition.y, this.chunkPosition.z), out neighbor))
            {
                Vector3Int neighborPoint = new Vector3Int(0, pos.y, pos.z);
                neighbor.UpdateTerrainAtPosition(neighborPoint, value);
                //UpdateSharedPointHelper(neighbor, neighborPoint, pos, value);
            }
        }
        // Upper edge, we share this point with one neighbor
        else if (pos.z == 0)
        {
            Chunk neighbor;
            // Get upper neighbor and update poiint at bottom edge
            if (this.neighboringChunks.TryGetValue(new Vector3Int(this.chunkPosition.x, this.chunkPosition.y, this.chunkPosition.z - width), out neighbor))
            {
                Vector3Int neighborPoint = new Vector3Int(pos.x, pos.y, width);
                neighbor.UpdateTerrainAtPosition(neighborPoint, value);
                //UpdateSharedPointHelper(neighbor, neighborPoint, pos, value);
            }

        }
        // Lower edge, we share this point with 1 neighboring chunk
        else if (pos.z == width)
        {
            Chunk neighbor;
            // Get lower neighbor and update poiint at upper edge
            if (this.neighboringChunks.TryGetValue(new Vector3Int(this.chunkPosition.x, this.chunkPosition.y, this.chunkPosition.z + width), out neighbor))
            {
                Vector3Int neighborPoint = new Vector3Int(pos.x, pos.y, 0);
                neighbor.UpdateTerrainAtPosition(neighborPoint, value);
                //UpdateSharedPointHelper(neighbor, neighborPoint, pos, value);
            }

        }
    }

    private void UpdateSharedPointHelper(Chunk neighbor, Vector3Int neighborPoint, Vector3Int localPoint, float value)
    {
        // We assume this chunk has already been updated by value
        float surfaceValue = this.SampleTerrain(localPoint);
        float neighborValue = neighbor.SampleTerrain(neighborPoint) + value;
        if (!neighborValue.Equals(surfaceValue))
            neighbor.UpdateTerrainAtPosition(neighborPoint, value);
    }

    public bool GetLeftNeighbor(out Chunk chunk)
    {
        return this.neighboringChunks.TryGetValue(new Vector3Int(this.chunkPosition.x - width, this.chunkPosition.y, this.chunkPosition.z), out chunk);
    }

    public bool GetUpperLeftNeighbor(out Chunk chunk)
    {
        return this.neighboringChunks.TryGetValue(new Vector3Int(this.chunkPosition.x - width, this.chunkPosition.y, this.chunkPosition.z - width), out chunk);
    }

    public bool GetUpperNeighbor(out Chunk chunk)
    {
        return this.neighboringChunks.TryGetValue(new Vector3Int(this.chunkPosition.x, this.chunkPosition.y, this.chunkPosition.z - width), out chunk);
    }

    public bool GetUpperRightNeighbor(out Chunk chunk)
    {
        return this.neighboringChunks.TryGetValue(new Vector3Int(this.chunkPosition.x + width, this.chunkPosition.y, this.chunkPosition.z - width), out chunk);
    }

    public bool GetRightNeighbor(out Chunk chunk)
    {
        return this.neighboringChunks.TryGetValue(new Vector3Int(this.chunkPosition.x + width, this.chunkPosition.y, this.chunkPosition.z), out chunk);
    }

    public bool GetLowerRightNeighbor(out Chunk chunk)
    {
        return this.neighboringChunks.TryGetValue(new Vector3Int(this.chunkPosition.x + width, this.chunkPosition.y, this.chunkPosition.z + width), out chunk);
    }

    public bool GetLowerNeighbor(out Chunk chunk)
    {
        return this.neighboringChunks.TryGetValue(new Vector3Int(this.chunkPosition.x, this.chunkPosition.y, this.chunkPosition.z + width), out chunk);
    }

    public bool GetLowerLeftNeighbor(out Chunk chunk)
    {
        return this.neighboringChunks.TryGetValue(new Vector3Int(this.chunkPosition.x - width, this.chunkPosition.y, this.chunkPosition.z + width), out chunk);
    }
}
