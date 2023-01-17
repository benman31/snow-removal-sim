using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public float timeLeft = 120f;
    public Text textBox;

    // Start is called before the first frame update
    void Start()
    {
        textBox.text = timeLeft.ToString();
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
