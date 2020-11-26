using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

public class GameManager : Singleton<GameManager>
{
    public static bool GameIsPaused;
    [SerializeField] private GameObject pauseMenuUI;

    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused) ResumeGame();
            else PauseGame();
        }
    }*/

    public void QuitGame() => Application.Quit();

    public void StartGame()
    {
        /*
         * TODO: this is a placeHolder, later I'll call smth like LevelManager.startLevel(FirstLevel)
         */
        SceneManager.LoadScene("GrowingApart");
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
}