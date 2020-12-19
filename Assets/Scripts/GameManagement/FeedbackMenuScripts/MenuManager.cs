using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;
using Debug = UnityEngine.Debug;

namespace GameManagement.FeedbackMenuScripts
{
    public class MenuManager : Singleton<MenuManager>
    {
        public enum Menu
        {
            Main,
            Levels,
            Settings,
            Feedback,
            About,
            Thanks
        };

        public GameObject MainMenu;
        public GameObject SettingsMenu;
        public GameObject LevelsMenu;
        public GameObject FeedbackMenu;
        public GameObject AboutMenu;
        public GameObject ThanksMenu;

        private void SetMenu(Menu menu)
        {
            MainMenu.SetActive(false);
            LevelsMenu.SetActive(false);
            SettingsMenu.SetActive(false);
            FeedbackMenu.SetActive(false);
            AboutMenu.SetActive(false);
            ThanksMenu.SetActive(false);

            switch (menu)
            {
                case Menu.Main:
                    MainMenu.SetActive(true);
                    break;
                case Menu.Settings:
                    SettingsMenu.SetActive(true);
                    break;
                case Menu.Levels:
                    LevelsMenu.SetActive(true);
                    break;
                case Menu.Feedback:
                    FeedbackMenu.SetActive(true);
                    break;
                case Menu.About:
                    AboutMenu.SetActive(true);
                    break;
                case Menu.Thanks:
                    ThanksMenu.SetActive(true);
                    break;
            }
        }

        public void Play()
        {
        }

        private void Start() => SetMenu(Menu.Feedback);

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) SetMenu(Menu.Feedback);
        }

        public void OpenThanksMenu() => SetMenu(Menu.Thanks);

        public void OpenMainMenu() => SetMenu(Menu.Main);
        public void OpenLevelsMenu() => SetMenu(Menu.Levels);
        public void OpenSettingsMenu() => SetMenu(Menu.Settings);
        public void OpenFeedbackMenu() => SetMenu(Menu.Feedback);
        public void OpenAboutMenu() => SetMenu(Menu.About);

        public void Submit()
        {
            Debug.Log("Submitting Feedback");
            SetMenu(Menu.Thanks);
        }

        public void ReturnToMainMenu()
        {
            Time.timeScale = 1.0f;
            SceneManager.LoadScene("MainMenu");
        }
    }
}