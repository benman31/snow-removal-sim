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

    OldGameGrid gameGrid;
    [SerializeField] private LayerMask whatIsAGridLayer;

    private OldGridCell highlightedGridCell;
    private HashSet<OldGridCell> highlightedCells;

    // Start is called before the first frame update
    void Start()
    {
        // Slow. Maybe make game grid a singleton instead?
        gameGrid = FindObjectOfType<OldGameGrid>();
        highlightedCells = new HashSet<OldGridCell> ();
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

        Vector2Int playerPos = this.gameGrid.GetGridPositionFromWorld(player.transform.position);

        OldGridCell cellMouseIsOver = IsMouseOverAGridCell();

        // Return the last highlighted gridcell to white
        if (highlightedGridCell != null && highlightedGridCell != cellMouseIsOver)
        {
            //highlightedGridCell.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
        }
        // Highlight the currently targeted grid cell
        if (highlightedCells.Count > 0)
        {
            //highlightedGridCell = cellMouseIsOver;

            //float distanceToCell = Vector3.Distance(highlightedGridCell.transform.position, player.transform.position);
           // if (distanceToCell > MAX_SHOVEL_RANGE)
           // {
                //cellMouseIsOver.GetComponentInChildren<MeshRenderer>().material.color = Color.red;

           // }
           // else
            //{
                //cellMouseIsOver.GetComponentInChildren<MeshRenderer>().material.color = Color.green;

                // Allow Shoveling if within range
                if (Input.GetMouseButtonDown(0))
                {
                    foreach (var cell in highlightedCells)
                    {
                        cell.ShovelSnow();
                    }
                }
            //}
        }
        Debug.Log($"highlighted cells: {this.highlightedCells.Count}");
    }

    // Return grid cell if mouse is over it, null otherwise
    private OldGridCell IsMouseOverAGridCell()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, whatIsAGridLayer))
        {

            //Vector3 cellPos = hitInfo.transform.GetComponent<OldGridCell>().transform.position;
            //Vector3 newPos = new Vector3(cellPos.x, cellPos.y, cellPos.z);
            //gridTargetting.transform.position = newPos;

            return hitInfo.transform.GetComponent<OldGridCell>();
        }
        else
        {
            return null;
        }

    }

    public void AddHighlightedCell(OldGridCell gridCell)
    {
        this.highlightedCells.Add(gridCell);
    }

    public void RemoveHighlightedCell(OldGridCell gridCell)
    {
        this.highlightedCells.Remove(gridCell);
    }
}
