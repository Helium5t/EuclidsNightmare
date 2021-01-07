using FMOD;
using FMOD.Studio;
using Utility;

namespace GameManagement
{
    public class GameMusic : Singleton<GameMusic>
    {
        private EventInstance _musicEventInstance;

        private void Start()
        {
            _musicEventInstance = FMODUnity.RuntimeManager.CreateInstance(GameSoundPaths.GameMusicSoundPath);
            _musicEventInstance.start();
            _musicEventInstance.release();
        }
    }
}