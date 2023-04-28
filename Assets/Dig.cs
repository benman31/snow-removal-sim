using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dig : MonoBehaviour
{
    public static event Action<Vector3> OnSnowCollision;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleSnowCollision(collision);

    }

    private void OnCollisionStay(Collision collision)
    {
        HandleSnowCollision(collision);

    }

    private void HandleSnowCollision(Collision collision)
    {
        if (collision.collider.tag == "Snow")
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                OnSnowCollision?.Invoke(contact.point);

            }
        }
    }
}
