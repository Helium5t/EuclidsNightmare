using System;
using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

namespace GameManagement
{
    public class GameMusic : Singleton<GameMusic>
    {
        private EventInstance _musicEventInstance;
        private const float _timeToResetMusicParameters = 0.5f;

        private const string TransitionParameter = "Transition";
        private const string ToMainMenuParameter = "ToMainMenu";

        private void OnEnable() => SceneManager.sceneLoaded += ManageMusicState;

        private void Start()
        {
            _musicEventInstance = RuntimeManager.CreateInstance(GameSoundPaths.GameMusicSoundPath);
            _musicEventInstance.start();

            #region InitBussesValues

            RuntimeManager.GetBus(GameBussesPaths.MasterBus).setVolume(1.0f);
            RuntimeManager.GetBus(GameBussesPaths.MusicBus).setVolume(0.5f);
            RuntimeManager.GetBus(GameBussesPaths.SFXBus).setVolume(0.5f);

            #endregion
        }

        private void ManageMusicState(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.name == Levels.MainMenu.ToString())
            {
                ChangeToMainMenuParameter(1);
                ChangeTransitionParameter(0);

                StartCoroutine(ResetMusicParameters(_timeToResetMusicParameters));
            }
            else
            {
                ChangeTransitionParameter(1);
                ChangeToMainMenuParameter(0);

                StartCoroutine(ResetMusicParameters(_timeToResetMusicParameters));
            }
        }

        private IEnumerator ResetMusicParameters(float time)
        {
            yield return new WaitForSeconds(time);
            ChangeTransitionParameter(0);
            ChangeToMainMenuParameter(0);
        }

        public void ChangeTransitionParameter(int value) =>
            _musicEventInstance.setParameterByName(TransitionParameter, value);

        public void ChangeToMainMenuParameter(int value) =>
            _musicEventInstance.setParameterByName(ToMainMenuParameter, value);

        private void OnDisable() => SceneManager.sceneLoaded -= ManageMusicState;
    }
}