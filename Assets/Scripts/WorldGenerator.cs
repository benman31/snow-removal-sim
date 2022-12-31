using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{

    public int WorldSizeInChunks = 10;
    public bool enableAccumulationOverTIme = false;
    public float updateTime = 1f;

    Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();
    
    // Start is called before the first frame update
    void Start()
    {
        Generate();
    }

    private void Update()
    {
        Vector3Int randomChunk = new Vector3Int(0, 0, 0);
        int randX = Mathf.FloorToInt(Random.Range(0f, WorldSizeInChunks + 0.9f)) * GameData.ChunkWidth;
        int randZ = Mathf.FloorToInt(Random.Range(0f, WorldSizeInChunks + 0.9f)) * GameData.ChunkWidth;
        randomChunk.x = randX;
        randomChunk.z = randZ;

        foreach (var chunk in chunks.Values)
        {
            if (this.enableAccumulationOverTIme) { 
                if (chunk.getChunkPosition().Equals(randomChunk))
                {
                    chunk.enableAccumulationOverTIme = true;
                }
                else
                {
                    chunk.enableAccumulationOverTIme = false;
                }
            }
            chunk.Update();
        }
    }

    void Generate ()
    {
        for (int x = 0; x < WorldSizeInChunks; x++)
        {
            for (int z = 0 ; z < WorldSizeInChunks; z++)
            {
                Vector3Int chunkPos = new Vector3Int(x * GameData.ChunkWidth, 0, z * GameData.ChunkWidth);
                chunks.Add(chunkPos, new Chunk(chunkPos, this));

                //Todo: Delete these probably

                //chunks[chunkPos].chunkObject.transform.SetParent(transform);
                //chunks[chunkPos].enableAccumulationOverTIme = this.enableAccumulationOverTIme;
                //chunks[chunkPos].updateTime = this.updateTime;
            }
        }

        // Set neihboring chunks
        foreach(var chunk in chunks.Values)
        {
            // Start debug here
            SetChunkNeighbors(chunk.getChunkPosition());
        }
    }

    public Chunk GetChunkFromVector3(Vector3 pos)
    {
        int x = (int)pos.x;
        int y = (int)pos.y;
        int z = (int)pos.z;

        Chunk chunk;
        if (chunks.TryGetValue(new Vector3Int(x, y, z), out chunk))
        {
            return chunk;
        }
        return null;
    }

    public Chunk GetChunkFromVector3Int(Vector3Int pos)
    {
        return chunks[pos];
    }

    /**
     * Set the neightboring chunks for a chunk at a given position in the chunks dictionary.
     * A chunk must already be constructed at the position to set its neighbors
     */
    private void SetChunkNeighbors(Vector3Int pos)
    {
        if (!chunks.ContainsKey(pos))
        {
            Debug.Log($"There is no chunk at position {pos}");
            return;
        }

        if (chunks.Count <= 1)
        {
            return;
        }

        int chunkWidth = GameData.ChunkWidth;
        int finalChunkPos = chunkWidth * (WorldSizeInChunks - 1);

        // All possible neighbors
        Vector3Int upperNeighbor = new Vector3Int(pos.x, pos.y, pos.z - chunkWidth);
        Vector3Int upperRightNeighbor = new Vector3Int(pos.x + chunkWidth, pos.y, pos.z - chunkWidth);
        Vector3Int rightNeighbor = new Vector3Int(pos.x + chunkWidth, pos.y, pos.z);
        Vector3Int lowerRightNeighbor = new Vector3Int(pos.x + chunkWidth, pos.y, pos.z + chunkWidth);
        Vector3Int lowerNeighbor = new Vector3Int(pos.x, pos.y, pos.z + chunkWidth);
        Vector3Int lowerLeftNeighbor = new Vector3Int(pos.x - chunkWidth, pos.y, pos.z + chunkWidth);
        Vector3Int leftNeighbor = new Vector3Int(pos.x - chunkWidth, pos.y, pos.z);
        Vector3Int upperLeftNeighbor = new Vector3Int(pos.x - chunkWidth, pos.y, pos.z - chunkWidth);

        // Corner Cases, we have 3 neighbors
        if (pos.x == 0 && pos.z == 0)
        {
            GetChunkFromVector3Int(pos).neighboringChunks.Add(rightNeighbor, GetChunkFromVector3Int(rightNeighbor));
            GetChunkFromVector3Int(pos).neighboringChunks.Add(lowerRightNeighbor, GetChunkFromVector3Int(lowerRightNeighbor));
            GetChunkFromVector3Int(pos).neighboringChunks.Add(lowerNeighbor, GetChunkFromVector3Int(lowerNeighbor));
        } 
        else if (pos.x == 0 && pos.z == finalChunkPos)
        {
            GetChunkFromVector3Int(pos).neighboringChunks.Add(upperNeighbor, GetChunkFromVector3Int(upperNeighbor));
            GetChunkFromVector3Int(pos).neighboringChunks.Add(upperRightNeighbor, GetChunkFromVector3Int(upperRightNeighbor));
            GetChunkFromVector3Int(pos).neighboringChunks.Add(rightNeighbor, GetChunkFromVector3Int(rightNeighbor));
        } 
        else if (pos.x == finalChunkPos && pos.z == 0)
        {
            GetChunkFromVector3Int(pos).neighboringChunks.Add(leftNeighbor, GetChunkFromVector3Int(leftNeighbor));
            GetChunkFromVector3Int(pos).neighboringChunks.Add(lowerLeftNeighbor, GetChunkFromVector3Int(lowerLeftNeighbor));
            GetChunkFromVector3Int(pos).neighboringChunks.Add(lowerNeighbor, GetChunkFromVector3Int(lowerNeighbor));
        } 
        else if (pos.x == finalChunkPos && pos.z == finalChunkPos)
        {
            GetChunkFromVector3Int(pos).neighboringChunks.Add(upperNeighbor, GetChunkFromVector3Int(upperNeighbor));
            GetChunkFromVector3Int(pos).neighboringChunks.Add(upperLeftNeighbor, GetChunkFromVector3Int(upperLeftNeighbor));
            GetChunkFromVector3Int(pos).neighboringChunks.Add(leftNeighbor, GetChunkFromVector3Int(leftNeighbor));
        }
        // Left edge, we have 5 neighbors
        else if (pos.x == 0)
        {
            GetChunkFromVector3Int(pos).neighboringChunks.Add(upperNeighbor, GetChunkFromVector3Int(upperNeighbor));
            GetChunkFromVector3Int(pos).neighboringChunks.Add(upperRightNeighbor, GetChunkFromVector3Int(upperRightNeighbor));
            GetChunkFromVector3Int(pos).neighboringChunks.Add(rightNeighbor, GetChunkFromVector3Int(rightNeighbor));
            GetChunkFromVector3Int(pos).neighboringChunks.Add(lowerRightNeighbor, GetChunkFromVector3Int(lowerRightNeighbor));
            GetChunkFromVector3Int(pos).neighboringChunks.Add(lowerNeighbor, GetChunkFromVector3Int(lowerNeighbor));
        }
        // Right edge, we have 5 neighbors
        else if (pos.x == finalChunkPos)
        {
            GetChunkFromVector3Int(pos).neighboringChunks.Add(upperNeighbor, GetChunkFromVector3Int(upperNeighbor));
            GetChunkFromVector3Int(pos).neighboringChunks.Add(upperLeftNeighbor, GetChunkFromVector3Int(upperLeftNeighbor));
            GetChunkFromVector3Int(pos).neighboringChunks.Add(leftNeighbor, GetChunkFromVector3Int(leftNeighbor));
            GetChunkFromVector3Int(pos).neighboringChunks.Add(lowerLeftNeighbor, GetChunkFromVector3Int(lowerLeftNeighbor));
            GetChunkFromVector3Int(pos).neighboringChunks.Add(lowerNeighbor, GetChunkFromVector3Int(lowerNeighbor));
        }
        // Upper edge, we have 5 neighbors
        else if (pos.z == 0)
        {
            GetChunkFromVector3Int(pos).neighboringChunks.Add(leftNeighbor, GetChunkFromVector3Int(leftNeighbor));
            GetChunkFromVector3Int(pos).neighboringChunks.Add(lowerLeftNeighbor, GetChunkFromVector3Int(lowerLeftNeighbor));
            GetChunkFromVector3Int(pos).neighboringChunks.Add(lowerNeighbor, GetChunkFromVector3Int(lowerNeighbor));
            GetChunkFromVector3Int(pos).neighboringChunks.Add(lowerRightNeighbor, GetChunkFromVector3Int(lowerRightNeighbor));
            GetChunkFromVector3Int(pos).neighboringChunks.Add(rightNeighbor, GetChunkFromVector3Int(rightNeighbor));
        }
        // Lower edge, we have 5 neighbors
        else if (pos.z == finalChunkPos)
        {
            GetChunkFromVector3Int(pos).neighboringChunks.Add(rightNeighbor, GetChunkFromVector3Int(rightNeighbor));
            GetChunkFromVector3Int(pos).neighboringChunks.Add(upperRightNeighbor, GetChunkFromVector3Int(upperRightNeighbor));
            GetChunkFromVector3Int(pos).neighboringChunks.Add(upperNeighbor, GetChunkFromVector3Int(upperNeighbor));
            GetChunkFromVector3Int(pos).neighboringChunks.Add(upperLeftNeighbor, GetChunkFromVector3Int(upperLeftNeighbor));
            GetChunkFromVector3Int(pos).neighboringChunks.Add(leftNeighbor, GetChunkFromVector3Int(leftNeighbor));
        }
        // Not an edge or a corner, we have 8 neightbors
        else
        {
            GetChunkFromVector3Int(pos).neighboringChunks.Add(rightNeighbor, GetChunkFromVector3Int(rightNeighbor));
            GetChunkFromVector3Int(pos).neighboringChunks.Add(upperRightNeighbor, GetChunkFromVector3Int(upperRightNeighbor));
            GetChunkFromVector3Int(pos).neighboringChunks.Add(upperNeighbor, GetChunkFromVector3Int(upperNeighbor));
            GetChunkFromVector3Int(pos).neighboringChunks.Add(upperLeftNeighbor, GetChunkFromVector3Int(upperLeftNeighbor));
            GetChunkFromVector3Int(pos).neighboringChunks.Add(leftNeighbor, GetChunkFromVector3Int(leftNeighbor));
            GetChunkFromVector3Int(pos).neighboringChunks.Add(lowerLeftNeighbor, GetChunkFromVector3Int(lowerLeftNeighbor));
            GetChunkFromVector3Int(pos).neighboringChunks.Add(lowerNeighbor, GetChunkFromVector3Int(lowerNeighbor));
            GetChunkFromVector3Int(pos).neighboringChunks.Add(lowerRightNeighbor, GetChunkFromVector3Int(lowerRightNeighbor));
        }
    }
}
