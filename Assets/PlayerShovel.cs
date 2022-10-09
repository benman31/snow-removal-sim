using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShovel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            //shoveling code
            Debug.Log("I am shovelling");
        }
    }
}
