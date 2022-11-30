using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{

    public int WorldSizeInChunks = 4;//10;

    //TEMP
    public float updateTime = 1f;

    Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();
    
    // Start is called before the first frame update
    void Start()
    {
        Generate();
    }

    private void Update()
    {
        /*updateTime -= Time.deltaTime;
        if (updateTime < 0)
        {
            foreach(var chunk in chunks.Values)
            {
                for(int x = 0; x <= GameData.ChunkWidth; x++)
                {
                    for(int z = 0; z <= GameData.ChunkWidth; z++)
                    {
                        float y = chunk.surfaceHeightMap[x, z];
                        chunk.AddTerrain(new Vector3Int(x, (int)y, z));
                        chunk.surfaceHeightMap[x, z]++;
                    }
                }

            }
            updateTime = 1f;
        }*/
    }

    void Generate ()
    {
        for (int x = 0; x < WorldSizeInChunks; x++)
        {
            for (int z = 0 ; z < WorldSizeInChunks; z++)
            {
                Vector3Int chunkPos = new Vector3Int(x * GameData.ChunkWidth, 0, z * GameData.ChunkWidth);
                chunks.Add(chunkPos, new Chunk(chunkPos));
                chunks[chunkPos].chunkObject.transform.SetParent(transform);
            }
        }

    }

    public Chunk GetChunkFromVector3(Vector3 pos)
    {
        int x = (int)pos.x;
        int y = (int)pos.y;
        int z = (int)pos.z;

        return chunks[new Vector3Int(x, y, z)];
    }
}
