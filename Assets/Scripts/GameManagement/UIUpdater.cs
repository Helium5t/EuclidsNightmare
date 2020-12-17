using UnityEngine;
using UnityEngine.UI;

public class UIUpdater : MonoBehaviour
{
    [SerializeField] private Text hintTextArea;
    [TextArea] [SerializeField] private string hintText;

    private GameObject levelLoaderGameObject;

    private void Awake()
    {
        UpdateUI();
        levelLoaderGameObject = GameObject.FindGameObjectWithTag("LevelLoader");
    }

    private void UpdateUI()
    {
        GameManager.Instance.UpdateUI(gameObject);
        hintTextArea.text = hintText;
        gameObject.SetActive(false);
    }

    public void PauseGame() => Time.timeScale = 0f;

    public void ResumeGame() => GameManager.Instance.ResumeGame();

    public void QuitGame() => GameManager.Instance.QuitGame();

    public void LoadMainMenu() => GameManager.Instance.LoadMainMenu();

    public void RestartCurrentLevel() => levelLoaderGameObject.GetComponent<LevelLoader>().RestartCurrentLevel();

    public void LoadNextLevel() => levelLoaderGameObject.GetComponent<LevelLoader>().LoadNextLevel();
}