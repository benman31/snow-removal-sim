using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PPEffectsController : MonoBehaviour
{
    [SerializeField] private Camera playerCam;

    private Wind wind;

    public float timeScale = 5.0f;

    private float timer = 0;
    private bool firstMelt = true;
    private bool melting;

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
                timer += (Time.deltaTime * dotP) / timeScale;
            }

            distort -= (Time.deltaTime * dotP) / timeScale;

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
            if(firstMelt)
            {
                firstMelt = false;
                playerCam.GetComponent<PostProcessingCamera>().waterShaderPass = 2;
            }

            yield return new WaitForSeconds(2); //wait 2 seconds before snow accumulation melts

            distort = timer;

            playerCam.GetComponent<PostProcessingCamera>().radius = Mathf.Lerp(1.0f, 0.0f, timer);

            playerCam.GetComponent<PostProcessingCamera>().feather = Mathf.Lerp(1.5f, 0.3f, timer);

            playerCam.GetComponent<PostProcessingCamera>().Intensity = Mathf.Lerp(0.5f, 1.5f, timer);

            if (timer >= 0)
            {
                timer -= Time.deltaTime / timeScale;
            }
        }

        if (!melting)
        {
            distort = 0;
        }

        playerCam.GetComponent<PostProcessingCamera>().distortion = Mathf.Lerp(0, 5.0f, distort / 2);
    }

    IEnumerator SpawnWaterDroplets()
    {
        playerCam.GetComponent<PostProcessingCamera>().distortion = Mathf.Lerp(0, 5.0f, timer);

        yield return null;
    }

    IEnumerator FadeWaterDroplets()
    {
        playerCam.GetComponent<PostProcessingCamera>().distortion = Mathf.Lerp(5.0f, 0, timer);

        yield return null;
    }

    IEnumerator AccumulateSnow()
    {
        yield return new WaitForSeconds(0); //wait 2 seconds before snow accumulation starts

        playerCam.GetComponent<PostProcessingCamera>().radius = Mathf.Lerp(1.5f, 0.0f, timer);

        playerCam.GetComponent<PostProcessingCamera>().feather = Mathf.Lerp(1.5f, 0.3f, timer);

        playerCam.GetComponent<PostProcessingCamera>().Intensity = Mathf.Lerp(0.5f, 1.5f, timer);

        if (timer < 1)
        {
            timer += Time.deltaTime / timeScale;
        }
    }

    IEnumerator MeltSnow()
    {
        yield return new WaitForSeconds(0); //wait 2 seconds before snow accumulation melts

        Debug.Log("i made it");

        playerCam.GetComponent<PostProcessingCamera>().radius = Mathf.Lerp(1.5f, 0.0f, timer);

        playerCam.GetComponent<PostProcessingCamera>().feather = Mathf.Lerp(1.5f, 0.3f, timer);

        playerCam.GetComponent<PostProcessingCamera>().Intensity = Mathf.Lerp(0.5f, 1.5f, timer);

        if (timer >= 0)
        {
            timer -= Time.deltaTime / timeScale;
        }
    }
}
