using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{
    [SerializeField] private GameObject levelLoader;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log(SceneManager.GetActiveScene().name + " COMPLETED!");
            // LevelChanger.Instance.FadeToNextLevel();
            levelLoader.GetComponent<LevelLoader>().LoadNextLevel();
        }
        else Debug.Log(SceneManager.GetActiveScene().name + " NOT completed");
    }
}