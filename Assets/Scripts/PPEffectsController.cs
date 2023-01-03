using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PPEffectsController : MonoBehaviour
{
    public float accumulationRate = 5.0f;
    public float meltingRate = 10.0f;

    [SerializeField] private Camera playerCam;

    private Wind wind;

    private float timer = 0;
    private bool firstMelt = true;
    private bool melting;
    private Vector2 windDirection;

    // Start is called before the first frame update
    void Start()
    {
        wind = GetComponentInChildren<Wind>();
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(PPEffects());
    }

    IEnumerator PPEffects()
    {
        float distort = 0;

        Vector2 windDir = wind.currentWindDir;
        Vector2 cameraFront = new Vector2(playerCam.transform.forward.x, playerCam.transform.forward.z);
        float dotP = Vector2.Dot(cameraFront, windDir);

        if (timer >= 0.5f)
        {
            melting = true;
        }

        Debug.Log("the dot product is  " + dotP);

        if (dotP > 0)
        {
            playerCam.GetComponent<PostProcessingCamera>().radius = Mathf.Lerp(1.0f, 0.0f, timer);

            playerCam.GetComponent<PostProcessingCamera>().feather = Mathf.Lerp(1.5f, 0.3f, timer);

            playerCam.GetComponent<PostProcessingCamera>().Intensity = Mathf.Lerp(0.5f, 1.5f, timer);

            if (timer < 1)
            {
                timer += (Time.deltaTime * dotP) / accumulationRate;
            }

            distort -= (Time.deltaTime * dotP) / accumulationRate;

            if (timer <= 0.4f) // stop screen from melting until a minimum amount of frost has accumulated
            {
                melting = false;
            }

            if(!firstMelt)
            {
                firstMelt = true;
            }
        }
        else
        {
            if(firstMelt) // adjust shader pass according to the amount of frost on the screen when melting starts after accumulating 
            {
                firstMelt = false;
                
                if(timer > 0.5f && timer < 0.6f)
                {
                    playerCam.GetComponent<PostProcessingCamera>().waterShaderPass = 0; // the higher the shader pass the more water droplets on the screen
                    playerCam.GetComponent<PostProcessingCamera>().timeScale = 2.0f;
                }

                else if(timer > 0.6f && timer < 0.8f)
                {
                    playerCam.GetComponent<PostProcessingCamera>().waterShaderPass = 1;
                    playerCam.GetComponent<PostProcessingCamera>().timeScale = 3.0f;
                }

                else if(timer > 0.8f)
                {
                    playerCam.GetComponent<PostProcessingCamera>().waterShaderPass = 2;
                    playerCam.GetComponent<PostProcessingCamera>().timeScale = 4.0f;
                }    
            }

            yield return new WaitForSeconds(2); //wait 2 seconds before snow accumulation melts

            playerCam.GetComponent<PostProcessingCamera>().radius = Mathf.Lerp(1.0f, 0.0f, timer);

            playerCam.GetComponent<PostProcessingCamera>().feather = Mathf.Lerp(1.5f, 0.3f, timer);

            playerCam.GetComponent<PostProcessingCamera>().Intensity = Mathf.Lerp(0.5f, 1.5f, timer);

            if (timer >= 0)
            {
                timer -= Time.deltaTime / meltingRate;
            }

            distort = timer;
        }

        if (!melting)
        {
            distort = 0;
        }

        playerCam.GetComponent<PostProcessingCamera>().distortion = Mathf.Lerp(0, 5.0f, distort);
    }
}
