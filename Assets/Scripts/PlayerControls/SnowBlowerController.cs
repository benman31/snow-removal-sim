using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

[RequireComponent(typeof(Rigidbody))]
public class SnowBlowerController : MonoBehaviour
{
    #region Variables
    private Rigidbody rb;
    private Vector3 previousPosition;
    private bool hasMoved;
    #endregion
    
    #region Event Actions
    public static event Action<Vector3> OnSnowCollision;
    public static event Action<bool> OnClearingSnow;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        PlayerTools.OnSnowblowerActive += HandleSnowblowerActive;
        rb = GetComponent<Rigidbody>();
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        PlayerTools.OnSnowblowerActive -= HandleSnowblowerActive;
    }

    void Update()
    {
        hasMoved = Vector3.SqrMagnitude(transform.position - previousPosition) > 0.0001f;

        previousPosition.x = transform.position.x;
        previousPosition.y = transform.position.y;
        previousPosition.z = transform.position.z;

        if (!hasMoved)
        {
            OnClearingSnow?.Invoke(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleSnowCollision(collision);

    }

    private void OnCollisionStay(Collision collision)
    {
        HandleSnowCollision(collision);
        
    }

    #region Custom Methods

    private void HandleSnowCollision(Collision collision)
    {
        if (collision.collider.tag == "Snow")
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                OnSnowCollision?.Invoke(contact.point);

            }
            
            if (hasMoved)
            {
                OnClearingSnow?.Invoke(true);
            }
        }
    }

    private void HandleSnowblowerActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
    #endregion
}
