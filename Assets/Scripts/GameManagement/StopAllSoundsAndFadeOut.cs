using FMOD.Studio;
using UnityEngine;
using Utility;

namespace GameManagement
{
    public class StopAllSoundsAndFadeOut : MonoBehaviour
    {
        private Bus _masterBus;

        private void Awake() => _masterBus = FMODUnity.RuntimeManager.GetBus(GameBussesPaths.MasterBus);
        
        private void OnDestroy() => _masterBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}