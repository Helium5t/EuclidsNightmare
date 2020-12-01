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

    public void LoadNextLevel() => StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));

    private void RestartCurrentLevel() => StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex));

    private IEnumerator LoadLevel(int levelBuildIndex)
    {
        animator.SetTrigger(Start);
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelBuildIndex);
        yield return null;
    }
}