using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SnowBlowerInput))]
public class SnowBlowerController : MonoBehaviour
{
    #region Variables
    private Rigidbody rb;
    private SnowBlowerInput input;
    [SerializeField] private Transform snowSpoutTransform;
    private float previousRotationY;
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
        rb = GetComponent<Rigidbody>();
        input = GetComponent<SnowBlowerInput>();

        gameObject.SetActive(false);

        PlayerTools.OnSnowblowerActive += HandleSnowblowerActive;
    }

    private void OnDestroy()
    {
        PlayerTools.OnSnowblowerActive -= HandleSnowblowerActive;
    }

    void Update()
    {
        Quaternion spoutRotation = snowSpoutTransform.localRotation * Quaternion.Euler(Vector3.up * 40 * input.RotationInput * Time.deltaTime);
        snowSpoutTransform.localRotation = ClampRotation(spoutRotation, new Vector3(0, 90f, 0));

        //bool hasRotated = Mathf.Abs(snowSpoutTransform.localEulerAngles.y - previousRotationY) <= 0.0001f;

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

    // Source: https://forum.unity.com/threads/how-do-i-clamp-a-quaternion.370041/
    public static Quaternion ClampRotation(Quaternion q, Vector3 bounds)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        angleX = Mathf.Clamp(angleX, -bounds.x, bounds.x);
        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        float angleY = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.y);
        angleY = Mathf.Clamp(angleY, -bounds.y, bounds.y);
        q.y = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleY);

        float angleZ = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.z);
        angleZ = Mathf.Clamp(angleZ, -bounds.z, bounds.z);
        q.z = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleZ);

        return q.normalized;
    }
}
