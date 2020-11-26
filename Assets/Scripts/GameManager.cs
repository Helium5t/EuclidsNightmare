using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Utility;

public class GameManager : Singleton<GameManager>
{
    public static bool GameIsPaused;
    private GameObject pauseMenuUI;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused) ResumeGame();
            else PauseGame();
        }
    }


    public void QuitGame() => Application.Quit();

    public void LoadLevel(string levelToLoad)
    {
        /*
         * TODO: this is a placeHolder, later I'll call smth like LevelManager.startLevel(FirstLevel)
         */
        SceneManager.LoadScene(levelToLoad);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        GameIsPaused = true;
        pauseMenuUI.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        GameIsPaused = false;
        pauseMenuUI.SetActive(false);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void UpdateUI(GameObject newUI)
    {
        Debug.Log("UpdateUI triggered with param: " + newUI.name);
        pauseMenuUI = newUI;
    }

    public void UpdateUI()
    {
        Debug.Log("UpdateUI triggered without param");
        pauseMenuUI = GameObject.FindGameObjectWithTag("Player").transform.Find("pauseMenuUI").gameObject;
    }
}