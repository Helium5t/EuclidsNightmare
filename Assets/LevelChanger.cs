using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

public class LevelChanger : Singleton<LevelChanger>
{
    public Animator animator;

    private static readonly int FadeOut = Animator.StringToHash("FadeOut");

    private int levelToLoad;

    public void FadeToNextLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    public void FadeToLevel(int levelIndex)
    {
        levelToLoad = levelIndex;
        animator.SetTrigger(FadeOut);
    }

    public void OnFadeComplete() => SceneManager.LoadScene(levelToLoad);
}