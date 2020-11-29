using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log(SceneManager.GetActiveScene().name + " COMPLETED!");
            LevelChanger.Instance.FadeToNextLevel();
        }
        else Debug.Log(SceneManager.GetActiveScene().name + " NOT completed");
    }
}