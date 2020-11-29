using UnityEngine;
using UnityEngine.UI;
using Utility;

public class ButtonClickerManager : MonoBehaviour
{
    [SerializeField] private Button[] _buttons;
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
            GameManager.Instance.LoadLevel(levelToLoad.ToString());
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