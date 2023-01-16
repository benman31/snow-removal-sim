/*
 * Author: Benjamin Enman, 97377
 * Based on the guide by MetalStorm Games: https://www.youtube.com/watch?v=qkSSdqOAAl4
 */

using UnityEngine;

public class GridCell// : MonoBehaviour
{
    private GameObject _gridCellObject;
    public GameObject gridCellObject 
    { 
        get 
        {
            return _gridCellObject;
        }
    }

    private Vector3Int position = new Vector3Int();

    private float maxSnowVolume;
    private float _snowVolume;
    public float snowVolume
    {
        get => _snowVolume;

        set
        {
            if (value >= 0f)
            {
                _snowVolume = value;
            }
        }
    }

    public GridCell(Vector3 pos, WorldGenerator world)
    {
        _gridCellObject = new GameObject();
        _gridCellObject.name = $"Grid Cell {pos.x}, {pos.z}";
        position.x = (int)pos.x;
        position.y = (int)pos.y;
        position.z = (int)pos.z;
        _gridCellObject.transform.position = position;

        this.maxSnowVolume = world.ChunkHeight;
    }


    // Update is called once per frame
    public void Update()
    {

    }

    // Get position in grid
    public Vector3Int GetPosition()
    {
        return position;
    }

    public bool IsClear()
    {
        return this.snowVolume <= 0f;
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Targetting"))
        {
            this.GetComponentInChildren<MeshRenderer>().material.color = Color.green;
            this.GetComponentInParent<GridInputManager>().AddHighlightedCell(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Targetting"))
        {
            this.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
            this.GetComponentInParent<GridInputManager>().RemoveHighlightedCell(this);
        }
    }*/
}
