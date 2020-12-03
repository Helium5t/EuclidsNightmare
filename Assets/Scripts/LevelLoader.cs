using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Utility;

public class LevelLoader : MonoBehaviour
{
    [Header("Exposed Parameters")] [SerializeField]
    private float sceneTransitionTime;

    [SerializeField] private float levelNameTransitionTime = 1f;

    [Space] [Header("Animators")] public Animator SceneAnimator;
    public Animator LevelNameAnimator;

    private static readonly int SceneTransitionTrigger = Animator.StringToHash("Start");
    private static readonly int LevelNameTransitionTrigger = Animator.StringToHash("NameFadeStart");

    private static Text _levelNameText;
    private static string _sceneName;
    private static int _sceneBuildIndex;

    private void Awake()
    {
        _sceneName = SceneManager.GetActiveScene().name;
        _sceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
        _levelNameText = GameObject.FindGameObjectWithTag("LevelNameText").gameObject.GetComponent<Text>();
    }

    public void LoadLevel(int levelBuildIndex) => StartCoroutine(LoadLevelRoutine(levelBuildIndex));

    public void LoadNextLevel() => StartCoroutine(LoadLevelRoutine(SceneManager.GetActiveScene().buildIndex + 1));

    public void RestartCurrentLevel() => StartCoroutine(LoadLevelRoutine(SceneManager.GetActiveScene().buildIndex));

    public void DisplayLevelName()
    {
        if (_sceneName == Levels.MainMenu.ToString() || _sceneName == Levels.EndDemoRoom.ToString()) return;
        _levelNameText.text = _sceneBuildIndex + ") " + _sceneName;
        StartCoroutine(LevelNameFade());
    }

    private IEnumerator LevelNameFade()
    {
        LevelNameAnimator.SetTrigger(LevelNameTransitionTrigger);
        yield return new WaitForSeconds(levelNameTransitionTime);
        LevelNameAnimator.SetTrigger(LevelNameTransitionTrigger);
        yield return null;
    }

    private IEnumerator LoadLevelRoutine(int levelBuildIndex)
    {
        Time.timeScale = 1f; //Just to be sure that everything is flowing as it should be
        SceneAnimator.SetTrigger(SceneTransitionTrigger);
        yield return new WaitForSeconds(sceneTransitionTime);
        SceneManager.LoadScene(levelBuildIndex);
        yield return null;
    }
}