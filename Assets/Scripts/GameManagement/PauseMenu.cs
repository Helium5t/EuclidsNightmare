using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameManagement
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private Text hintTextArea;
        [TextArea] [SerializeField] private string hintText;

        private GameObject levelLoaderGameObject;

        public static bool GameIsPaused;
        private GameObject settingsMenu;
        
        //private static readonly int PauseMenuFadeOut = Animator.StringToHash("PauseMenuFadeOut");

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GameIsPaused) ResumeGame();
                else PauseGame();
            }
        }

        private void Awake()
        {
            UpdateUI();
            levelLoaderGameObject = GameObject.FindGameObjectWithTag("LevelLoader");
            settingsMenu = GameObject.FindGameObjectWithTag("SettingsMenu");
            settingsMenu.SetActive(false);
        }

        private void UpdateUI()
        {
            GameManager.Instance.UpdateUI(gameObject);
            hintTextArea.text = hintText;
            gameObject.SetActive(false);
        }

        public void ResumeGame()
        {
            Time.timeScale = 1f;
            GameIsPaused = false;
            gameObject.SetActive(false);
            settingsMenu.SetActive(false);
        }
        
        public void PauseGame()
        {
            Time.timeScale = 0f;
            GameIsPaused = true;
            gameObject.SetActive(true);
        }
        
        public void QuitGame() => GameManager.Instance.QuitGame();

        public void LoadMainMenu() => GameManager.Instance.LoadMainMenu();

        public void RestartCurrentLevel() => levelLoaderGameObject.GetComponent<LevelLoader>().RestartCurrentLevel();

        public void LoadNextLevel() => levelLoaderGameObject.GetComponent<LevelLoader>().LoadNextLevel();
        //public void PlayPauseMenuAnimation(bool fadeInOrOut) => GetComponent<Animator>().SetBool(PauseMenuFadeOut, fadeInOrOut);
    }
}