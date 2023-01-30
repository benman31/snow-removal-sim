using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveManager : MonoBehaviour
{
    private Dictionary<string, Objective> _objectives = new Dictionary<string, Objective>();
    [SerializeField] private GameObject objectiveHighlight;
    [SerializeField] private GameGrid gameGrid;

    [SerializeField] private Canvas objectiveDisplay;

    private MeshRenderer[] highlights; 

    public bool isDirty = false;
    private int objectivesComplete = 0;

    private bool showHighlightedObjectives = false;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = gameGrid.transform.position;
        this.transform.SetParent(gameGrid.transform, true);

        Objective[] objs = this.GetComponentsInChildren<Objective>();
        for (int i = 0; i < objs.Length; i++)
        {
            Objective obj = objs[i];
            obj.transform.SetParent(this.transform, false);
            for (int x = obj.xStart; x < obj.xEnd; x++)
            {
                for (int z = obj.zStart; z < obj.zEnd; z++)
                {
                    GameObject highlight = Instantiate(objectiveHighlight, new Vector3(gameGrid.transform.position.x + (float)x + 0.5f, gameGrid.transform.position.y + 0.5f, gameGrid.transform.position.z + (float)z + 0.5f ), Quaternion.identity);
                    highlight.GetComponentInChildren<MeshRenderer>().enabled = false;
                    highlight.transform.SetParent(this.transform, false);
                    //highlight.transform.localScale = gameGrid.transform.parent.localScale;
                    obj.objectiveHighlights.Add(highlight);
                }
            }
            AddObjective(obj.name, obj);

        }
        this.transform.localScale = new Vector3(gameGrid.transform.localScale.x, 10, gameGrid.transform.localScale.y);
        Debug.Log($"Objective Count: {_objectives.Count}");

        this.objectiveDisplay.enabled = false;
        this.highlights = this.GetComponentsInChildren<MeshRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        if (isDirty)
        {
            int completeCount = 0;
            foreach (Objective obj in _objectives.Values)
            {
                if (obj.IsComplete())
                {
                    completeCount++;
                }
            }
            this.objectivesComplete = completeCount;
            this.isDirty = false;
        }

        if (Input.GetKeyDown(KeyCode.Tab) && !showHighlightedObjectives)
        {
            foreach (MeshRenderer mesh in this.highlights)
            {
                mesh.enabled = true;
            }
            objectiveDisplay.enabled = true;
            showHighlightedObjectives = true;
        }

        if (Input.GetKeyUp(KeyCode.Tab) && showHighlightedObjectives)
        {
            foreach (MeshRenderer mesh in this.highlights)
            {
                mesh.enabled = false;
            }
            objectiveDisplay.enabled = false;
            showHighlightedObjectives = false;
        }
    }

    public bool AddObjective(string objID, Objective obj)
    {
        return _objectives.TryAdd(objID, obj);
    }

    public bool GetObjective(string objId, out Objective obj)
    {
        return _objectives.TryGetValue(objId, out obj);
    }

    public bool AddToCompletion(GridCell gridCell)
    {
        bool result = false;

        foreach(Objective obj in _objectives.Values)
        {
            if (obj.GridCellIsObjective(gridCell))
            {
                obj.AddCompleteCell(gridCell);
                result = true;
            }
        }
        
        return result;
    }

    public bool RemoveFromCompletion(GridCell gridCell)
    {
        bool result = false;

        foreach (Objective obj in _objectives.Values)
        {
            if (obj.GridCellIsObjective(gridCell))
            {
                obj.RemoveCompleteCell(gridCell);
                result = true;
            }
        }

        return result;
    }

    public int GetObjectiveCount()
    {
        return _objectives.Count;
    }

    public bool AllObjectivesComplete()
    {
        return this.objectivesComplete >= this.GetObjectiveCount();
    }

    /*private string PrintSummary()
    {
        string result = $"Objectives {this.objectivesComplete}/{this.GetObjectiveCount()}";
        foreach (Objective obj in _objectives.Values)
        {
            result += $"- {obj.PrintPrettyString()}\n";
        }
        return result;
    }*/
}
