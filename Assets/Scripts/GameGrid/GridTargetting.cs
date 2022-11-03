/*
 * Author: Benjamin Enman, 97377
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTargetting : MonoBehaviour
{
    [SerializeField] private Transform ground;

    // Keep a reference to the original position of the targetting transform in a sibling game object
    [SerializeField] private Transform originalTargettingTransform;


    // Start is called before the first frame update
    void Start()
    {
        // Set local transform to the original position of the targetting collider (this will not update) 
        originalTargettingTransform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, this.transform.localPosition.z);
        originalTargettingTransform.localRotation = this.transform.localRotation;
        originalTargettingTransform.localScale = new Vector3(this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z);
    }

    // Update is called once per frame
    void Update()
    {
        // Source for rotation: https://answers.unity.com/questions/1231974/quad-to-rotate-towards-camera-on-y-axis-only.html
        Vector3 upAxis = Vector3.up;
        transform.rotation = Quaternion.LookRotation(Vector3.Cross(upAxis, Vector3.Cross(upAxis, Camera.main.transform.forward)), upAxis);
        
        if (originalTargettingTransform.position.y <= ground.position.y)
        {
            Vector3 clampedPosition = new Vector3(originalTargettingTransform.position.x, Mathf.Max(originalTargettingTransform.position.y, ground.position.y), originalTargettingTransform.position.z);
            transform.position = clampedPosition;
        }
        else if (transform.localPosition.x != originalTargettingTransform.localPosition.x || transform.localPosition.y != originalTargettingTransform.localPosition.y || transform.localPosition.z != originalTargettingTransform.localPosition.z)
        {
            transform.localPosition = new Vector3(originalTargettingTransform.localPosition.x, originalTargettingTransform.localPosition.y, originalTargettingTransform.localPosition.z);
        }
    } 
}
