using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void LoadGame()
    {
        //SceneManager.UnloadSceneAsync("MainMenu");
        SceneManager.LoadScene("MVPScene");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting the game...");
        Application.Quit();
    }
}
