using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private float transitionTime;

    public Animator animator;

    private static readonly int Start = Animator.StringToHash("Start");

    public void LoadLevel(int levelBuildIndex) => StartCoroutine(LoadLevelRoutine(levelBuildIndex));

    public void LoadNextLevel() => StartCoroutine(LoadLevelRoutine(SceneManager.GetActiveScene().buildIndex + 1));

    public void RestartCurrentLevel() => StartCoroutine(LoadLevelRoutine(SceneManager.GetActiveScene().buildIndex));

    private IEnumerator LoadLevelRoutine(int levelBuildIndex)
    {
        Time.timeScale = 1f; //Just to be sure that everything is flowing as it should be
        animator.SetTrigger(Start);
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelBuildIndex);
        yield return null;
    }
}