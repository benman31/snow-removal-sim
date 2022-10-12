/*
 * Author: Benjamin Enman, 97377
 * Based on the guide by MetalStorm Games: https://www.youtube.com/watch?v=qkSSdqOAAl4
 */
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridInputManager : MonoBehaviour
{
    const float MAX_SHOVEL_RANGE = 4f;

    GameGrid gameGrid;
    [SerializeField] private LayerMask whatIsAGridLayer;

    private GridCell highlightedGridCell;

    // Start is called before the first frame update
    void Start()
    {
        // Slow. Maybe make game grid a singleton instead?
        gameGrid = FindObjectOfType<GameGrid>();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject player = FindObjectOfType<PlayerMovement>().gameObject;
        // Don't update until player has loaded in from main scene
        if (player == null)
        {
            return;
        }

        Vector2 playerPos = this.gameGrid.GetGridPositionFromWorld(player.transform.position);
        Debug.Log($"Player transform x: {player.transform.position.x}, y: {player.transform.position.z}");
        Debug.Log($"Player x: {playerPos.x}, y: {playerPos.y}");

        GridCell cellMouseIsOver = IsMouseOverAGridCell();

        // Return the last highlighted gridcell to white
        if (highlightedGridCell != null && highlightedGridCell != cellMouseIsOver)
        {
            highlightedGridCell.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
        }
        // Highlight the currently targeted grid cell
        if (cellMouseIsOver)
        {
            highlightedGridCell = cellMouseIsOver;

            float distanceToCell = Vector3.Distance(highlightedGridCell.transform.position, player.transform.position);
            if (distanceToCell > MAX_SHOVEL_RANGE)
            {
                cellMouseIsOver.GetComponentInChildren<MeshRenderer>().material.color = Color.red;

            }
            else
            {
                cellMouseIsOver.GetComponentInChildren<MeshRenderer>().material.color = Color.green;
                
                // Allow Shoveling if within range
                if (Input.GetMouseButtonDown(0))
                {
                    cellMouseIsOver.ShovelSnow();
                }
            }
        }
        
    }

    // Return grid cell if mouse is over it, null otherwise
    private GridCell IsMouseOverAGridCell()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, whatIsAGridLayer))
        {
            return hitInfo.transform.GetComponent<GridCell>();
        }
        else
        {
            return null;
        }

    }

    private List<Vector2> GetGridCellPositionsAroundTarget(GridCell targetCell, int numCellsLeft, int numCellsRight, int numCellsTop, int numCellsBottom)
    {
        Vector2 targetPos = targetCell.GetPosition();

        List<Vector2> neighboringCells = new List<Vector2>();

        return neighboringCells;
    }
}
