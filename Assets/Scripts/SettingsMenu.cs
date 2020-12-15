using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMOD.Studio;
using Utility;

public class SettingsMenu : MonoBehaviour
{
    public Dropdown resolutions;
    public Dropdown graphicsQualities;

    private Resolution[] possibleResolutions;

    private EventInstance SFXVolumeTestEvent;
    private Bus masterBus;
    private Bus musicBus;
    private Bus SFXbus;

    // Set these values the same as the UI Sliders!
    private float MasterVolume = 1f;
    private float MusicVolume = 0.5f;
    private float SFXVolume = 0.5f;

    private void Awake()
    {
        masterBus = FMODUnity.RuntimeManager.GetBus(GameBussesPaths.MasterBus);
        musicBus = FMODUnity.RuntimeManager.GetBus(GameBussesPaths.MusicBus);
        SFXbus = FMODUnity.RuntimeManager.GetBus(GameBussesPaths.SFXBus);
        SFXVolumeTestEvent = FMODUnity.RuntimeManager.CreateInstance(GameSoundPaths.SfxVolumeTestPath);
    }

    private void Update()
    {
        masterBus.setVolume(MasterVolume);
        musicBus.setVolume(MusicVolume);
        SFXbus.setVolume(SFXVolume);
    }

    private void Start()
    {
        graphicsQualities.GetComponent<Dropdown>().value = QualitySettings.GetQualityLevel();
        UpdateResolutionsDropdown();
    }

    public void setDisableAudio(bool isAudioDisabled) => masterBus.setMute(isAudioDisabled);

    public void setMasterVolume(float newMasterVolume) => MasterVolume = newMasterVolume;

    public void setMusicBusVolume(float newMusicBusVolume) => MusicVolume = newMusicBusVolume;

    public void SetSfxVolume(float newSfxVolume)
    {
        SFXVolume = newSfxVolume;

        SFXVolumeTestEvent.getPlaybackState(out PLAYBACK_STATE playbackState);
        if (playbackState != PLAYBACK_STATE.PLAYING) SFXVolumeTestEvent.start();
    }

    public void setResolution(int resolutionIndex)
    {
        Resolution resolution = possibleResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void setVsync(bool isVsyncActive) => QualitySettings.vSyncCount = isVsyncActive ? 1 : 0;

    public void setFullScreen(bool isFullScreen) => Screen.fullScreen = isFullScreen;

    public void changeGraphicsQuality(int graphicsQualityIndex) =>
        QualitySettings.SetQualityLevel(graphicsQualityIndex);

    private void UpdateResolutionsDropdown()
    {
        possibleResolutions = Screen.resolutions;

        List<string> resolutionsStringList = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < possibleResolutions.Length; i++)
        {
            resolutionsStringList.Add(possibleResolutions[i].width + " x " + possibleResolutions[i].width + " @" +
                                      possibleResolutions[i].refreshRate);

            if (possibleResolutions[i].width == Screen.currentResolution.width &&
                possibleResolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutions.AddOptions(resolutionsStringList);
        resolutions.value = currentResolutionIndex;
        resolutions.RefreshShownValue();
    }
}