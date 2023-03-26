using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SnowBlowerInput))]
public class SnowBlowerController : MonoBehaviour
{
    #region Variables
    [Header("Movement Properties")]
    public float movementSpeed = 15f;
    public float rotationSpeed = 20f;

    private Rigidbody rb;
    private SnowBlowerInput input;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        input = GetComponent<SnowBlowerInput>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(rb && input)
        {
            HandleMovement();
        }
    }

    #region Custom Methods
    protected virtual void HandleMovement()
    {
        Vector3 desiredPosition = transform.position + (transform.forward * input.ForwardInput * movementSpeed * Time.deltaTime);
        rb.MovePosition(desiredPosition);

        Quaternion desiredRotation = transform.rotation * Quaternion.Euler(Vector3.up * rotationSpeed * input.RotationInput * Time.deltaTime);
        rb.MoveRotation(desiredRotation);
    }
    #endregion
}
