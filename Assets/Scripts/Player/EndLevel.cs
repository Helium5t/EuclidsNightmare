using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player
{
    public class EndLevel : MonoBehaviour
    {
        [SerializeField] private GameObject exitDoor;
        private TimerDoorTrigger timerDoorTrigger;
        
        private void Start() => timerDoorTrigger = exitDoor.GetComponent<TimerDoorTrigger>();

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Door"))
            {
                if (timerDoorTrigger.triggered)
                {
                    Debug.Log(SceneManager.GetActiveScene().name + " COMPLETED!");
                    LevelChanger.Instance.FadeToNextLevel();
                }
                else Debug.Log(SceneManager.GetActiveScene().name + " NOT completed");
            }
        }
    }
}