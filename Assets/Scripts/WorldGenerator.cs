using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class WorldGenerator : MonoBehaviour
{
    public int WorldSizeInChunks = 10;
    public int ChunkWidth = 10;
    public int ChunkHeight = 70;
    public float scale = 1f;

    public float BaseTerrainHeight = 0.0f;
    public float TerrainHeightRange = 5f;

    public bool enableAccumulationOverTime = false;
    public float updateTime = 1f;

    private float timeAcc = 0f;
    
    [SerializeField] private GameGrid _gameGrid;
    [SerializeField] private WeatherController _weatherController;
    public GameGrid gameGrid
    {
        get { return _gameGrid; }
    }

    Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();
    
    // Keeping a separate linked list of chunks to make updating easier for snow accumulation over time
    private LinkedList<Chunk> chunkList = new LinkedList<Chunk>();
    private LinkedListNode<Chunk> currentUpdateChunk;
    
    // Start is called before the first frame update
    void Start()
    {
        this.transform.localScale *= scale;
    }

    private void Awake()
    {
        Generate();
        currentUpdateChunk = chunkList.First;
    }

    private void Update()
    {
        timeAcc += Time.deltaTime;

        float clampedWindDirX = 0f;
        float clampedWindDirZ = 0f;

        if (this.enableAccumulationOverTime && timeAcc >= updateTime)
        {
            Vector2 windVector = _weatherController.wind.currentWindDir;
            float angle = Mathf.Atan2(windVector.y, windVector.x);
            angle = ((int)(Mathf.Round(4 * angle / Mathf.PI + 8)) % 8) * Mathf.PI / 4;
            clampedWindDirX = Mathf.Cos(angle);
            clampedWindDirZ = Mathf.Sin(angle);
        }

        bool advanceUpdateChunk = false;
        foreach (var chunk in chunks.Values)
        {
            if (this.enableAccumulationOverTime && timeAcc >= updateTime)
            {
                if (chunk.Equals(this.currentUpdateChunk.Value))
                {
                    chunk.GrowOverTime(new Vector2Int((int)clampedWindDirX, (int)clampedWindDirZ), _weatherController.wind.windIntesity);
                    advanceUpdateChunk = true;
                }
            }
            chunk.Update();
        }
        if (advanceUpdateChunk)
        {
            if (currentUpdateChunk.Next != null)
            {
                currentUpdateChunk = currentUpdateChunk.Next;
            }
            else
            {
                currentUpdateChunk = chunkList.First;
            }
            advanceUpdateChunk = false;
        }

        timeAcc = 0f;
    }

    void Generate ()
    {
        for (int x = 0; x < WorldSizeInChunks; x++)
        {
            for (int z = 0 ; z < WorldSizeInChunks; z++)
            {
                Vector3Int chunkPos = new Vector3Int(x * ChunkWidth, 0, z * ChunkWidth);
                chunks.Add(chunkPos, new Chunk(chunkPos, this));
                chunks[chunkPos].chunkObject.transform.SetParent(transform);
                chunkList.AddLast(chunks[chunkPos]);
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
        int x = Mathf.FloorToInt(pos.x / ChunkWidth) * ChunkWidth;
        int y = 0;
        int z = Mathf.FloorToInt(pos.z / ChunkWidth) * ChunkWidth;
        Chunk chunk;
        if (chunks.TryGetValue(new Vector3Int(x, y, z), out chunk))
        {
            return chunk;
        }
        return null;
    }
    public Chunk GetChunkFromVectorXYZ(float x, float y, float z)
    {
        int posX = (int)x;
        int posY = (int)y;
        int posZ = (int)z;

        Chunk chunk;
        if (chunks.TryGetValue(new Vector3Int(posX, posY, posZ), out chunk))
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

        int chunkWidth = this.ChunkWidth;
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
    // Testing / Prototype
    private Chunk GetRandomChunkFromWindDirection(Vector2Int windDir)
    {
        float xMin = 0f;
        float xMax = 0f;
        float zMin = 0f;
        float zMax = 0f;
        int worldWidth = WorldSizeInChunks * ChunkWidth - 1;

        if (windDir.x == 1 && windDir.y == 0)
        {
            // East
            xMin = worldWidth / 2;
            xMax = worldWidth;
            zMin = 0;
            zMax = worldWidth;
        }
        else if (windDir.x == 1 && windDir.y == 1)
        {
            // Southeast
            xMin = worldWidth / 2;
            xMax = worldWidth;
            zMin = worldWidth / 2;
            zMax = worldWidth;
        }
        else if (windDir.x == 0 && windDir.y == 1)
        {
            // South
            xMin = 0;
            xMax = worldWidth;
            zMin = worldWidth / 2;
            zMax = worldWidth;
        }
        else if (windDir.x == -1 && windDir.y == 1)
        {
            // SouthWest
            xMin = 0;
            xMax = worldWidth / 2;
            zMin = worldWidth / 2;
            zMax = worldWidth;
        }
        else if (windDir.x == -1 && windDir.y == 0)
        {
            // West
            xMin = 0;
            xMax = worldWidth / 2;
            zMin = 0;
            zMax = worldWidth;
        }
        else if (windDir.x == -1 && windDir.y == -1)
        {
            // NorthWest
            xMin = 0;
            xMax = worldWidth / 2;
            zMin = 0;
            zMax = worldWidth / 2;
        }
        else if (windDir.x == 0 && windDir.y == -1)
        {
            // North
            xMin = 0;
            xMax = worldWidth;
            zMin = 0;
            zMax = worldWidth / 2;
        }
        else if (windDir.x == 1 && windDir.y == -1)
        {
            // NorthEast
            xMin = worldWidth / 2;
            xMax = worldWidth;
            zMin = 0;
            zMax = worldWidth / 2;
        }
        
        Vector3 ranomVector = new Vector3(Random.Range(xMin, xMax), 0, Random.Range(zMin, zMax));
        return GetChunkFromVector3(ranomVector);
    }
}
