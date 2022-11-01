/*
 * Author: Benjamin Enman, 97377
 * Based on the guide by MetalStorm Games: https://www.youtube.com/watch?v=qkSSdqOAAl4
 */
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GameGrid : MonoBehaviour
{
    public int height = 10;
    public int width = 10;
    public float gridCellSize = 1f;
    
    [SerializeField] private GameObject gridCellPrefab;
    private GameObject[,] gameGrid;
    private float gridOffsetX;
    private float gridOffsetZ;

    // Start is called before the first frame update
    void Start()
    {
        gridOffsetX = this.width / 2 * gridCellSize;
        gridOffsetZ = this.height / 2 * gridCellSize;

        CreateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Creates grid when game starts
    private void CreateGrid()
    {
        gameGrid = new GameObject[width, height];

        if (gridCellPrefab == null)
        {
            Debug.LogError("Error: Grid Cell Prefab not assigned");
            return;
        }

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                gameGrid[x, z] = Instantiate(gridCellPrefab, new Vector3(x * gridCellSize, 0, z * gridCellSize), Quaternion.identity);
                gameGrid[x, z].GetComponent<GridCell>().SetPosition(x, z);
                gameGrid[x, z].transform.parent = this.transform;
                gameGrid[x, z].gameObject.name = $"Grid Cell (x: {x}, z: {z})";

                // Scale the grid cell for debug visuals
                gameGrid[x, z].GetComponent<GridCell>().Scale(gridCellSize);

            }
        }
        this.transform.Translate(new Vector3(-gridOffsetX, 0,  -gridOffsetZ));
    }

    // Currently broken we may want to consider shifting our game world to positive coordinates only
    public Vector2Int GetGridPositionFromWorld(Vector3 worldPos)
    {
        // Todo: try adding offset to world pos before dividing
        int x = Mathf.FloorToInt((worldPos.x + gridOffsetX) / gridCellSize);
        int z = Mathf.FloorToInt((worldPos.z + gridOffsetZ) / gridCellSize);

        x = Mathf.Clamp(x, 0, width);
        z = Mathf.Clamp(z, 0, height);

        return new Vector2Int(x, z);
    }

    public Vector3 GetWorldPosFromGridPos(Vector2Int gridPos)
    {
        float x = gridPos.x * gridCellSize;
        float z = gridPos.y * gridCellSize;

        return new Vector3(x, 0, z);
    }
}
