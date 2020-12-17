using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMOD.Studio;
using Utility;

public class SettingsMenu : MonoBehaviour
{
    public Dropdown resolutions;
    public Dropdown graphicsQualities;

    private Resolution[] _possibleResolutions;

    private EventInstance _sfxVolumeTestEvent;
    private Bus _masterBus;
    private Bus _musicBus;
    private Bus _sfxBus;

    // Set these values the same as the UI Sliders!
    private float _masterVolume = 1f;
    private float _musicVolume = 0.5f;
    private float _sfxVolume = 0.5f;

    private void Awake()
    {
        _masterBus = FMODUnity.RuntimeManager.GetBus(GameBussesPaths.MasterBus);
        _musicBus = FMODUnity.RuntimeManager.GetBus(GameBussesPaths.MusicBus);
        _sfxBus = FMODUnity.RuntimeManager.GetBus(GameBussesPaths.SFXBus);
        _sfxVolumeTestEvent = FMODUnity.RuntimeManager.CreateInstance(GameSoundPaths.SfxVolumeTestPath);
    }

    private void Update()
    {
        _masterBus.setVolume(_masterVolume);
        _musicBus.setVolume(_musicVolume);
        _sfxBus.setVolume(_sfxVolume);
    }

    private void Start()
    {
        graphicsQualities.GetComponent<Dropdown>().value = QualitySettings.GetQualityLevel();
        UpdateResolutionsDropdown();
    }

    public void ChangeFovValue(float fovValue) => Camera.main.fieldOfView = fovValue;

    public void SetDisableAudio(bool isAudioDisabled) => _masterBus.setMute(isAudioDisabled);

    public void SetMasterVolume(float newMasterVolume) => _masterVolume = newMasterVolume;

    public void SetMusicBusVolume(float newMusicBusVolume) => _musicVolume = newMusicBusVolume;

    public void SetSfxBusVolume(float newSfxVolume)
    {
        _sfxVolume = newSfxVolume;

        _sfxVolumeTestEvent.getPlaybackState(out PLAYBACK_STATE playbackState);
        if (playbackState != PLAYBACK_STATE.PLAYING) _sfxVolumeTestEvent.start();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = _possibleResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetVsync(bool isVsyncActive) => QualitySettings.vSyncCount = isVsyncActive ? 1 : 0;

    public void SetFullScreen(bool isFullScreen) => Screen.fullScreen = isFullScreen;

    public void ChangeGraphicsQuality(int graphicsQualityIndex) =>
        QualitySettings.SetQualityLevel(graphicsQualityIndex);

    private void UpdateResolutionsDropdown()
    {
        _possibleResolutions = Screen.resolutions;

        List<string> resolutionsStringList = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < _possibleResolutions.Length; i++)
        {
            resolutionsStringList.Add(_possibleResolutions[i].width + " x " + _possibleResolutions[i].width + " @" +
                                      _possibleResolutions[i].refreshRate);

            if (_possibleResolutions[i].width == Screen.currentResolution.width &&
                _possibleResolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutions.AddOptions(resolutionsStringList);
        resolutions.value = currentResolutionIndex;
        resolutions.RefreshShownValue();
    }
}