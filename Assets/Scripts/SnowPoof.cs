/*
Written by: Abdelrahman Awad
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowPoof : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       Destroy(this.gameObject, 3.0f);
    }
}
