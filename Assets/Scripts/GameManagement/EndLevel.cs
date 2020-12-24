using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameManagement
{
    [RequireComponent(typeof(AudioSource))]
    public class EndLevel : MonoBehaviour
    {
        [SerializeField] private GameObject levelLoader;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log(SceneManager.GetActiveScene().name + " COMPLETED!");
                PlayEndLevelClip();
                levelLoader.GetComponent<LevelLoader>().LoadNextLevel();
            }
            else Debug.Log(SceneManager.GetActiveScene().name + " NOT completed");
        }

        private void PlayEndLevelClip()
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Sounds/EndLevelSound/EndLevelSound",
                gameObject.transform.position);
        }
    }
}