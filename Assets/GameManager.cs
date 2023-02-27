// Author: Ben Enman
// Source: State management and event structure based loosley on this tutorial by Tarodev: https://www.youtube.com/watch?v=4I0vonyqMi8

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    Paused,
    Unpaused,
    Win,
    Lose,
    MainMenu,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState State;
    public static event Action<GameState> OnGameStateChanged;

    [SerializeField] private TimeManager timeManager;
    [SerializeField] private ObjectiveManager objectiveManager;
    [SerializeField] private Text endMissionTextBox;

    private string gameOverMsg = "Time's up! You lose!";
    private string winMsg = "Objectives clear! You win!!";

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        endMissionTextBox.text = "";
        // Todo Change to main menu when as default state once main menu is implemented
        UpdateGameState(GameState.Unpaused);
    }

    // Update is called once per frame
    void Update()
    {
        if (timeManager.TimeIsUp() && !objectiveManager.AllObjectivesComplete())
        {
            endMissionTextBox.text = gameOverMsg;
        }
        else if (objectiveManager.AllObjectivesComplete() && !timeManager.TimeIsUp())
        {
            endMissionTextBox.text = winMsg;
        }
    }

    public void UpdateGameState(GameState newState)
    {
        State = newState;

        switch (newState)
        {
            case GameState.Paused:
                HandlePaused();
                break;
            case GameState.Unpaused:
                break;
            case GameState.Win:
                break;
            case GameState.Lose:
                break;
            case GameState.MainMenu:
                break;

           default:
                throw new System.ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChanged?.Invoke(newState);
    }

    private void HandlePaused()
    {

    }
}
