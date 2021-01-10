using FMOD.Studio;
using Utility;

namespace GameManagement
{
    public class GameMusic : Singleton<GameMusic>
    {
        private EventInstance _musicEventInstance;

        private const string TransitionParameter = "Transition";
        private const string ToMainMenuParameter = "ToMainMenu";

        private void Start()
        {
            _musicEventInstance = FMODUnity.RuntimeManager.CreateInstance(GameSoundPaths.GameMusicSoundPath);
            _musicEventInstance.start();
        }

        public void ChangeTransitionParameter(int value) =>
            _musicEventInstance.setParameterByName(TransitionParameter, value);

        public void ChangeToMainMenuParameter(int value) => 
            _musicEventInstance.setParameterByName(ToMainMenuParameter, value); 
    }
}