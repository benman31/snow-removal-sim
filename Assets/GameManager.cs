using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private Objective objective;

    [SerializeField] private Text endMissionTextBox;

    private string gameOverMsg = "Time's up! You lose!";
    private string winMsg = "Objectives clear! You win!!";

    // Start is called before the first frame update
    void Start()
    {
        endMissionTextBox.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (timeManager.TimeIsUp() && !objective.IsComplete())
        {
            endMissionTextBox.text = gameOverMsg;
        }
        else if (objective.IsComplete() && !timeManager.TimeIsUp())
        {
            endMissionTextBox.text = winMsg;
        }
    }
}
