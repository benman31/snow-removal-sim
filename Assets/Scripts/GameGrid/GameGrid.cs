/*
 * Author: Benjamin Enman, 97377
 * Based on the guide by MetalStorm Games: https://www.youtube.com/watch?v=qkSSdqOAAl4
 */
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GameGrid : MonoBehaviour
{
    private GridCell[,] gameGrid;

    private WorldGenerator world;

    [SerializeField] private Objective objective;

    private int width = 10;

    private Dictionary<string, Chunk> dirtyChunks;

    private const float VOLUME_UPDATE_FREQ = 0.5f;
    private float lastUpdateTime = 0f;

    
    // Start is called before the first frame update
    void Start()
    {
        world = this.GetComponentInParent<WorldGenerator>();
        this.name = "GameGrid";
        this.transform.position = world.transform.position;
        this.transform.parent = world.transform;

        this.width = world.ChunkWidth * world.WorldSizeInChunks;

        dirtyChunks = new Dictionary<string, Chunk>();

        CreateGrid();
    }

    // Update is called once per frame
    public void Update()
    {
        // Recompute volume once per second
        float now = Time.realtimeSinceStartup;
        if (now - lastUpdateTime > VOLUME_UPDATE_FREQ)
        {
            foreach (Chunk chunk in dirtyChunks.Values)
            {
                Vector3Int chunkPos = chunk.getChunkPosition();
                for (int x = chunkPos.x; x < chunkPos.x + world.ChunkWidth; x++)
                {
                    for (int z = chunkPos.z; z < chunkPos.z + world.ChunkWidth; z++)
                    {
                        float newSnowVolume = 0f;
                        for (int y = 0; y < world.ChunkHeight; y++)
                        {
                            Vector3Int cubePosition = new Vector3Int(x, y, z) - chunkPos;
                            float cubeVolume = chunk.GetCubeVolume(cubePosition);
                            newSnowVolume += cubeVolume;
                        }

                        GridCell gridCell = gameGrid[x, z];

                        bool wasClear = gridCell.IsClear();

                        gridCell.snowVolume = newSnowVolume;

                        if (!wasClear && gridCell.IsClear())
                        {
                            Debug.Log($"Cell{x}, {z} is Clear!!");
                            if (objective.IsObjective(gridCell))
                            {
                                objective.AddCompleteCell(gridCell);
                            }
                        }
                        else if (wasClear && !gridCell.IsClear())
                        {
                            if (objective.IsObjective(gameGrid[x, z]))
                            {
                                objective.RemoveCompleteCell(gridCell);
                            }
                        }
                    }
                }
            }
            dirtyChunks.Clear();

            lastUpdateTime = now;
        }
    }

    // Creates grid when game starts and computes the volume of snow sitting above each cell
    private void CreateGrid()
    {
        gameGrid = new GridCell[width, width];

        for (int z = 0; z < width; z++)
        {
            for (int x = 0; x < width; x++)
            {
                GridCell gridCell = gameGrid[x, z] = new GridCell(new Vector3(x, 0, z), world);
                gridCell.gridCellObject.transform.parent = this.transform;

                Chunk chunk = GetChunkFromGridCell(gridCell);
                Vector3Int chunkPos = chunk.getChunkPosition();

                float snowVolume = 0f;
                for (int y = 0; y < world.ChunkHeight; y++)
                {
                    Vector3Int cubePosition = new Vector3Int(x, y, z) - chunkPos;
                    float cubeVolume = chunk.GetCubeVolume(cubePosition);
                    snowVolume += cubeVolume;
                }
                gridCell.snowVolume = snowVolume;

            }
        }
    }

    // Add chunk to to list of chunks that need to have volume recalculated
    public void AddChunkToDirtyList(Chunk chunk)
    {
        dirtyChunks.TryAdd(chunk.chunkObject.name, chunk);
    }

    // Get the chunk this cell belongs to
    public Chunk GetChunkFromGridCell(GridCell gridCell)
    {
        int chunkX = Mathf.FloorToInt(gridCell.GetPosition().x / world.ChunkWidth) * world.ChunkWidth;
        int chunkZ = Mathf.FloorToInt(gridCell.GetPosition().z / world.ChunkWidth) * world.ChunkWidth;
        return world.GetChunkFromVectorXYZ(chunkX, 0f, chunkZ);
    }

    public Vector2Int GridSpaceToChunkSpace(float gridX, float gridY, float gridZ)
    {
        int chunkX = Mathf.FloorToInt(gridX / world.ChunkWidth) * world.ChunkWidth;
        int chunkZ = Mathf.FloorToInt(gridZ / world.ChunkWidth) * world.ChunkWidth;

        Vector2Int pointInChunk = new Vector2Int((int)gridX - chunkX, (int)gridZ - chunkZ);
        return pointInChunk;
    }

    public Vector3Int GridPointToChunkPoint(float gridX, float gridY, float gridZ)
    {
        int chunkX = Mathf.FloorToInt(gridX / world.ChunkWidth) * world.ChunkWidth;
        int chunkZ = Mathf.FloorToInt(gridZ / world.ChunkWidth) * world.ChunkWidth;

        Vector3Int pointInChunk = new Vector3Int((int)gridX - chunkX, (int)gridY, (int)gridZ - chunkZ);
        return pointInChunk;
    }

}
