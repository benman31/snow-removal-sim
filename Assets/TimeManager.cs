using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public float timeLeft = 120f;
    public Text textBox;

    [SerializeField] private TimeController timeController;

    [Range(0, 24.0f)] public float missionStart;
    [Range(0, 24.0f)] public float missionEnd;

    [Range(0, 24.0f)] [SerializeField] private float sunRise;
    [Range(0, 24.0f)] [SerializeField] private float sunSet; 

    [Tooltip("the duration of the full day in real life minutes")]
    [SerializeField] private float dayLength = 24.0f;   

    // Start is called before the first frame update
    void Start()
    {
        timeLeft = missionEnd - missionStart;
        timeLeft *= dayLength * 2.5f;
        
        textBox.text = timeLeft.ToString();

        timeController.SetStartHour(missionStart);
        timeController.SetSunRiseHour(sunRise);
        timeController.SetSunSetHour(sunSet);
        timeController.setTimeMultiplier(1440/dayLength);

        timeController.gameObject.SetActive(true);
        timeController.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            timeLeft = Mathf.Max(timeLeft, 0f);
            textBox.text = $"{SecondsToPrettyString()}";
        }
    }

    public bool TimeIsUp()
    {
        return timeLeft <= 0f;
    }

    private string SecondsToPrettyString()
    {
        int minutes = Mathf.FloorToInt(timeLeft / 60);
        int seconds = Mathf.FloorToInt(timeLeft % 60);

        string minuteStr = minutes < 10 ? $"0{minutes}" : minutes.ToString();
        string secondStr = seconds < 10 ? $"0{seconds}" : seconds.ToString();

        return $"{minuteStr}:{secondStr}";
    }
}
