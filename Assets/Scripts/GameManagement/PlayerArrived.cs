using UnityEngine;


namespace GameManagement
{
    /// <summary>
    /// This simple script is only used in the ForcedPerspectiveIntro Scene. :=)
    /// </summary>
    public class PlayerArrived : MonoBehaviour
    {

        [SerializeField] private GameObject door;
        private static readonly int doorAnimatorBool = Animator.StringToHash("open");

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")) door.GetComponent<Animator>().SetBool(doorAnimatorBool, true);
        }
    }
}
