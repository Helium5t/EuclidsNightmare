using UnityEngine;

public class UIUpdater : MonoBehaviour
{
    private void Awake()
    {
        GameManager.Instance.UpdateUI(gameObject);
        gameObject.SetActive(false);
    }

    public void ResumeGame() => GameManager.Instance.ResumeGame();
    public void QuitGame() => GameManager.Instance.QuitGame();
    public void LoadMainMenu() => GameManager.Instance.LoadMainMenu();
    public void RestartCurrentLevel() => GameManager.Instance.RestartCurrentLevel();
}