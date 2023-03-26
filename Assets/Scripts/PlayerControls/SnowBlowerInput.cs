using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBlowerInput : MonoBehaviour
{
    #region Variables
    [Header("Input Properties")]
    public Camera camera;
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

    #endregion

    #region Builtin Methods
    void Update()
    {
        if (camera)
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
        Ray screenRay =  camera.ViewportPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(screenRay, out hit))
        {
            _reticlePosition = hit.point;
            _normal = hit.normal;
        }

        _forwardInput = Input.GetAxis("Vertical");
        _rotationInput = Input.GetAxis("Horizontal");
    }
    #endregion
}
