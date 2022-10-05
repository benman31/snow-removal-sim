using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridInputManager : MonoBehaviour
{
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
        GridCell cellMouseIsOver = IsMouseOverAGridCell();

        if (highlightedGridCell != null && highlightedGridCell != cellMouseIsOver)
        {
            highlightedGridCell.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
        }

        if (cellMouseIsOver)
        {
            highlightedGridCell = cellMouseIsOver;

            cellMouseIsOver.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
            
            if (Input.GetMouseButtonDown(0))
            {
                cellMouseIsOver.ShovelSnow();
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
}
