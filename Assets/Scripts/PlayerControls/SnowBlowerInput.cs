using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBlowerInput : MonoBehaviour
{
    #region Variables
    [Header("Input Properties")]
    #endregion

    #region Properties
    private Vector3 _reticlePosition;
    public Vector3 ReticlePosition
    {
        get { return _reticlePosition; }
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

    public static event Action<bool> OnSpoutRotation;

    #endregion

    #region Builtin Methods
    private void Start()
    {
        PlayerTools.OnSnowblowerActive += HandleSnowblowerActive;
    }

    private void OnDestroy()
    {
        PlayerTools.OnSnowblowerActive -= HandleSnowblowerActive;
    }
    void Update()
    {
        if (SnowblowerActive)
        {
            HandleInputs();
        }
    }

    #endregion

    #region Custom Methods
    protected virtual void HandleInputs()
    {   
        _rotationInput = 0f;
        
        if (Input.GetKey(KeyCode.Q))
        {
            _rotationInput = -1f;
        }

        if (Input.GetKey(KeyCode.E))
        {
            _rotationInput = 1f;
        }

        if (Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.E))
        {
            _rotationInput = 0f;
        }

        if (_rotationInput == 0f)
        {
            OnSpoutRotation?.Invoke(false);
        }
        else
        {
            OnSpoutRotation?.Invoke(true);
        }
    }

    private void HandleSnowblowerActive(bool isActive)
    {
        _snowblowerActive = isActive;
    }
    #endregion
}
