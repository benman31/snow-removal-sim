using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindTrailController : MonoBehaviour
{
    public GameObject windTrail;
    public float spawnDelay;

    public Vector2 worldEdgesXZ;

    private WeatherController weatherController;
    private float spawnTimer;
    // Start is called before the first frame update
    void Start()
    {
        weatherController = GetComponent<WeatherController>();
        spawnTimer = spawnDelay;
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer -= Time.deltaTime;

        if(spawnTimer <= 0)
        {
            spawnTimer = spawnDelay;

            float x = Random.Range(-worldEdgesXZ.x, worldEdgesXZ.x + 1.0f);
            float z = Random.Range(-worldEdgesXZ.y, worldEdgesXZ.y + 1.0f);

            GameObject trail = Instantiate(windTrail, new Vector3(x, windTrail.transform.position.y, z), windTrail.transform.rotation);
            trail.GetComponent<Rigidbody>().AddForce(weatherController.wind.currentWindDir * Mathf.SmoothStep(30, 60, weatherController.windIntesity/200));
        }
    }
}
