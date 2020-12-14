using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Dropdown resolutions;
    public AudioMixer audioMixer;

    private Resolution[] possibleResolutions;

    private void Start()
    {
        possibleResolutions = Screen.resolutions;

        List<string> resoStringList = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < possibleResolutions.Length; i++)
        {
            resoStringList.Add(possibleResolutions[i].width + " x " + possibleResolutions[i].width);

            if (possibleResolutions[i].width == Screen.currentResolution.width &&
                possibleResolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutions.AddOptions(resoStringList);
        resolutions.value = currentResolutionIndex;
        resolutions.RefreshShownValue();
    }

    public void setResolution(int resolutionIndex)
    {
        Resolution resolution = possibleResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void setVolume(float volumeVal) => audioMixer.SetFloat("MasterVolume", volumeVal);
    public void setVsync(bool isVsyncActive) => QualitySettings.vSyncCount = isVsyncActive ? 1 : 0;

    public void setFullScreen(bool isFullScreen) => Screen.fullScreen = isFullScreen;

    public void changeGraphicsQuality(int graphicsQualityIndex) =>
        QualitySettings.SetQualityLevel(graphicsQualityIndex);
}