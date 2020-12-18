﻿using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

namespace GameManagement
{
    public class GameManager : Singleton<GameManager>
    {
        public static bool GameIsPaused;
        public int targetFPS = 60;
    
        private GameObject pauseMenuUI;
        private GameObject settingsMenuUI;

        private void Start() => Application.targetFrameRate = targetFPS;

        private void Update()
        {
            // Might be an over kill but it'll work!
            if (Application.targetFrameRate != targetFPS) Application.targetFrameRate = targetFPS;

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
            settingsMenuUI.SetActive(false);
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
            settingsMenuUI = GameObject.FindGameObjectWithTag("SettingsMenu");
        }
        

        public void UpdateUI()
        {
            Debug.Log("UpdateUI triggered without param");
            pauseMenuUI = GameObject.FindGameObjectWithTag("PauseMenu");
            settingsMenuUI = GameObject.FindGameObjectWithTag("SettingsMenu");
        }
    }
}