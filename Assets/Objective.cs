using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{
    [SerializeField, Range(0, 200)] private int xStart;
    [SerializeField, Range(0, 200)] private int xEnd;

    [SerializeField, Range(0, 200)] private int zStart;
    [SerializeField, Range(0, 200)] private int zEnd;

    [SerializeField] float percentageToComplete = 1f;

    private int objectiveGoal;
    private int cellsComplete;

    // Start is called before the first frame update
    void Start()
    {
        objectiveGoal = ((xEnd - xStart) + 1) * ((zEnd - zStart) + 1);
        cellsComplete = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.IsComplete())
        {
            Debug.Log("OBJECTIVE COMPLETE!!!!");
        }
    }

    public void AddCompleteCell(GridCell gridCell)
    {
        int x = gridCell.GetPosition().x;
        int z = gridCell.GetPosition().z;

        if (x < xStart || x > xEnd || z < zStart || z > zEnd)
        {
            Debug.LogWarning("Attempted to mark invalid cell coordinate as complete");
            return;
        }
        if (cellsComplete >= objectiveGoal)
        {
            Debug.LogWarning("We have exceeded 100% completion, something is wrong");
            return;
        }
        cellsComplete++;
    }

    public void RemoveCompleteCell(GridCell gridCell)
    {
        int x = gridCell.GetPosition().x;
        int z = gridCell.GetPosition().z;

        if (x < xStart || x > xEnd || z < zStart || z > zEnd)
        {
            Debug.LogWarning("Attempted to remove invalid cell coordinate");
            return;
        }
        if (cellsComplete <= 0)
        {
            Debug.LogWarning("We are trying to go below 0% completion, something is wrong");
            return;
        }
        cellsComplete--;
    }

    public bool IsObjective(GridCell gridCell)
    {
        return gridCell.GetPosition().x >= this.xStart && gridCell.GetPosition().x <= this.xEnd
            && gridCell.GetPosition().z >= this.zStart && gridCell.GetPosition().z <= this.zEnd;
    }

    public bool IsComplete()
    {
        return (float)cellsComplete / (float)objectiveGoal >= percentageToComplete;
    }


}
