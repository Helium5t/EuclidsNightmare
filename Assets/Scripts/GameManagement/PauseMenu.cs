using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameManagement
{
    public class PauseMenu : MonoBehaviour
    {
        #region PrivateVariables

        [SerializeField] private Text hintTextArea;
        [TextArea] [SerializeField] private string hintText;
        [SerializeField] private GameObject settingsMenu;
        [SerializeField] private GameObject pauseButtons;

        private GameObject levelLoaderGameObject;
        private Animator pauseMenuAnimator;

        #endregion

        public static bool GameIsPaused;

        private static readonly int PauseMenuAnimationOn = Animator.StringToHash("PauseMenuAnim");
        private static readonly int PauseMenuAnimationOff = Animator.StringToHash("PauseMenuIdle");

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
            levelLoaderGameObject = GameObject.FindGameObjectWithTag("LevelLoader");
            pauseMenuAnimator = GetComponent<Animator>();

            settingsMenu.SetActive(false);
            pauseButtons.SetActive(false);

            hintTextArea.text = hintText;
        }

        public void ResumeGame()
        {
            Time.timeScale = 1f;
            GameIsPaused = false;
            pauseMenuAnimator.Play(PauseMenuAnimationOff);
            pauseButtons.SetActive(false);
            settingsMenu.SetActive(false);
        }

        public void PauseGame()
        {
            Time.timeScale = 0f;
            GameIsPaused = true;
            pauseButtons.SetActive(true);
            pauseMenuAnimator.Play(PauseMenuAnimationOn);
        }

        public void QuitGame() => GameManager.Instance.QuitGame();

        public void LoadMainMenu() => levelLoaderGameObject.GetComponent<LevelLoader>().LoadLevel(0);

        public void RestartCurrentLevel() => levelLoaderGameObject.GetComponent<LevelLoader>().RestartCurrentLevel();

        public void LoadNextLevel() => levelLoaderGameObject.GetComponent<LevelLoader>().LoadNextLevel();
    }
}