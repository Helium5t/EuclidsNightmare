using System;
using FMOD.Studio;
using UnityEngine;
using Utility;

public class CubeSoundsManager : MonoBehaviour
{
    private EventInstance cubeSoundInstance;

    private void Start()
    {
        cubeSoundInstance = FMODUnity.RuntimeManager.CreateInstance(GameSoundPaths.CrateFallSoundPath);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(cubeSoundInstance, gameObject.transform,
            GetComponent<Rigidbody>());
    }

    private void OnCollisionEnter(Collision other) => PlayCrateHitSound();

    private void PlayCrateHitSound()
    {
        cubeSoundInstance.getPlaybackState(out PLAYBACK_STATE cubeState);

        if (cubeState != PLAYBACK_STATE.PLAYING)
        {
            cubeSoundInstance.start();
            cubeSoundInstance.setVolume(0.5f);
        }
    }
    private void OnDestroy() => cubeSoundInstance.release();
}