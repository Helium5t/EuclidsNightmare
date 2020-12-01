using UnityEngine;
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

    public void PauseGame()
    {
        Time.timeScale = 0f;
        GameIsPaused = true;
        pauseMenuUI.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        pauseMenuUI.SetActive(false);
    }

    // TODO: this could be modified with a LevelLoader.loadLevel(0) call...
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
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