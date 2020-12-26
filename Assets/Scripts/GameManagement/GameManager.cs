using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

namespace GameManagement
{
    public class GameManager : Singleton<GameManager>
    {
        public int targetFPS = 60;


        private void Start() => Application.targetFrameRate = targetFPS;

        private void Update()
        {
            if (Application.targetFrameRate != targetFPS) Application.targetFrameRate = targetFPS;
        }

        public void QuitGame() => Application.Quit();

        public void LoadMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }
    }
}