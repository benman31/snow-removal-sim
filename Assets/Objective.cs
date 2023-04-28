// Author: Benjamin Enman 97377

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Objective : MonoBehaviour
{
    private ObjectiveManager manager;

    [SerializeField, Range(0, 200)] private int _xStart;
    [SerializeField, Range(0, 200)] private int _xEnd;

    public int xStart { get { return _xStart; } }
    public int xEnd { get { return _xEnd; } }

    [SerializeField, Range(0, 200)] private int _zStart;
    [SerializeField, Range(0, 200)] private int _zEnd;

    public int zStart { get { return _zStart; } }
    public int zEnd { get { return _zEnd; } }

    [SerializeField] float percentageToComplete = 1f;
    [SerializeField] float volumeClearThreshold = 1f;

    private bool isDirty = false;

    private int _objectiveGoal;
    private int _cellsComplete;

    [SerializeField] private string _objectiveName  = "";
    [SerializeField] private string _description = "";
    [SerializeField] private bool _isOptional = false;
    [SerializeField] private float _reward = 0f;
    [SerializeField] private float _timeLimit = -1f;

    public string objectiveName { get => _objectiveName; }
    public string description { get => _description; }
    public bool isOptional { get => _isOptional; }
    public float reward { get => _reward; }   
    public float timeLimit { get => _timeLimit; }

    [HideInInspector] public List<GameObject> objectiveHighlights = new List<GameObject>();

    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private TextMeshProUGUI bodyText;

    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponentInParent<ObjectiveManager>();
        _objectiveGoal = ((xEnd - xStart) + 1) * ((zEnd - zStart) + 1);
        _cellsComplete = 0;
        headerText.text = this.name;
        bodyText.text = this.description;
        bodyText.richText = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.isDirty)
        {
            this.UpdateProgressText();
            this.isDirty = false;
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
        if (_cellsComplete >= _objectiveGoal)
        {
            Debug.LogWarning("We have exceeded 100% completion, something is wrong");
            return;
        }
        _cellsComplete++;
        manager.isDirty = true;
        this.isDirty = true;
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
        if (_cellsComplete <= 0)
        {
            Debug.LogWarning("We are trying to go below 0% completion, something is wrong");
            return;
        }
        _cellsComplete--;
        manager.isDirty = true;
        this.isDirty = true;
    }

    public bool GridCellIsObjective(GridCell gridCell)
    {
        return gridCell.GetPosition().x >= this.xStart && gridCell.GetPosition().x <= this.xEnd
            && gridCell.GetPosition().z >= this.zStart && gridCell.GetPosition().z <= this.zEnd;
    }

    public bool IsComplete()
    {
        return (float)_cellsComplete / (float)_objectiveGoal >= percentageToComplete;
    }

    public void UpdateProgressText()
    {
        string percentComplete = Mathf.FloorToInt((float)_cellsComplete / (float)_objectiveGoal * 100).ToString();
        this.headerText.text = $"{this.objectiveName} ({percentComplete}%)";
    }

}
