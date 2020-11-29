using UnityEngine;
using UnityEngine.UI;
using Utility;

public class ButtonClickerManager : MonoBehaviour
{
    [SerializeField] private Button[] _buttons;

    [Space] [SerializeField] private bool loadSpecificLevel;
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

            if (loadSpecificLevel) GameManager.Instance.LoadLevel(levelToLoad.ToString());
            else LevelChanger.Instance.FadeToNextLevel();
        }

        if (pressedButton == _buttons[1])
        {
            Debug.Log("Clicked " + _buttons[1].name);
            GameManager.Instance.QuitGame();
        }
    }

    private void OnDisable()
    {
        foreach (Button button in _buttons) button.onClick.RemoveAllListeners();
    }
}