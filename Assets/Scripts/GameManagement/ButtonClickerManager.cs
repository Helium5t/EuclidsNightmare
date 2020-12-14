using System;
using UnityEngine;
using UnityEngine.UI;
using Utility;

public class ButtonClickerManager : MonoBehaviour
{
    [SerializeField] private Button[] _buttons;

    [Header("Level Loader Game Object")] [Space] [SerializeField]
    private GameObject levelLoaderGO;

    [Header("Here if you want to load a specific level")] [Space] [SerializeField]
    private bool loadSpecificLevel;

    [SerializeField] private Levels levelToLoad;

    private void OnEnable()
    {
        foreach (Button button in _buttons) button.onClick.AddListener(() => ButtonCallback(button));
    }

    private void ButtonCallback(Button pressedButton)
    {
        if (pressedButton == _buttons[0])
        {
            Debug.Log("Clicked " + _buttons[0].name);

            if (loadSpecificLevel) levelLoaderGO.GetComponent<LevelLoader>().LoadLevel((int) levelToLoad);
            else levelLoaderGO.GetComponent<LevelLoader>().LoadNextLevel();
        }

        if (pressedButton == _buttons[1])
        {
            Debug.Log("Clicked " + _buttons[1].name);
            Application.OpenURL("https://forms.gle/ZkGGFC5y2kofv2ha9");
            GameManager.Instance.QuitGame();
        }
    }

    private void OnDisable()
    {
        foreach (Button button in _buttons) button.onClick.RemoveAllListeners();
    }
}