/*
Written by: Abdelrahman Awad
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootFlames : MonoBehaviour
{
    private ParticleSystem[] particleSystems;

    // Start is called before the first frame update
    void Start()
    {
        particleSystems = this.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem pSys in particleSystems)
        {
            pSys.enableEmission = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            foreach (ParticleSystem pSys in particleSystems)
            {
                pSys.enableEmission = true;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            foreach (ParticleSystem pSys in particleSystems)
            {
                pSys.enableEmission = false;
            }
        }
    }
}
