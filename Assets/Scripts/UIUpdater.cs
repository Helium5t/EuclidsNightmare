﻿using UnityEngine;

public class UIUpdater : MonoBehaviour
{
    private GameObject levelLoaderGameObject;
    
    private void Awake()
    {
        UpdateUI();
        levelLoaderGameObject = GameObject.FindGameObjectWithTag("LevelLoader");
    }

    private void UpdateUI()
    {
        GameManager.Instance.UpdateUI(gameObject);
        gameObject.SetActive(false);
    }

    public void ResumeGame() => GameManager.Instance.ResumeGame();

    public void QuitGame() => GameManager.Instance.QuitGame();

    public void LoadMainMenu() => GameManager.Instance.LoadMainMenu();

    public void RestartCurrentLevel() => levelLoaderGameObject.GetComponent<LevelLoader>().RestartCurrentLevel();

    public void LoadNextLevel() => levelLoaderGameObject.GetComponent<LevelLoader>().LoadNextLevel();
}