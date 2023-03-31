using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBlowerInput : MonoBehaviour
{
    /*#region Variables
    [Header("Input Properties")]
    public Camera cam;
    [SerializeField] private Transform playerHandTransform;
    #endregion

    #region Properties
    private Vector3 _reticlePosition;
    public Vector3 ReticlePosition
    {
        get { return _reticlePosition; }
    }

    private Vector3 _normal;
    public Vector3 Normal
    {
        get { return _normal; }
    }

    private float _forwardInput;
    public float ForwardInput
    {
        get { return _forwardInput; }
    }

    private float _rotationInput;
    public float RotationInput
    {
        get { return _rotationInput; }
    }

    [Header("State Properties")]
    
    private bool _snowblowerActive;
    public bool SnowblowerActive
    {
        get { return _snowblowerActive; }
    }

    #endregion

    #region Builtin Methods
    private void Start()
    {
        //PlayerTools.OnSnowblowerActive += HandleSnowblowerActive;
       //gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        //PlayerTools.OnSnowblowerActive -= HandleSnowblowerActive;
    }
    void Update()
    {
        if (cam && SnowblowerActive)
        {
            HandleInputs();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(ReticlePosition, 0.5f);
    }

    #endregion

    #region Custom Methods
    protected virtual void HandleInputs()
    {
        Ray screenRay =  cam.ViewportPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(screenRay, out hit))
        {
            _reticlePosition = hit.point;
            _normal = hit.normal;
        }

        _forwardInput = Input.GetAxis("Vertical");
        _rotationInput = Input.GetAxis("Horizontal");
    }

    private void HandleSnowblowerActive(bool isActive)
    {
        _snowblowerActive = isActive;
        if (isActive)
        {
            transform.parent = null;
            gameObject.SetActive(true);
        }
        else
        {
            transform.parent = playerHandTransform;
            gameObject.SetActive(false);
        }
    }
    #endregion
    */
}
