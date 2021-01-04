using UnityEngine;

namespace Utility
{
    public class DoorSoundsManager : MonoBehaviour
    {
        public void PlayOpenDoorSound() =>
            FMODUnity.RuntimeManager.PlayOneShot("event:/Sounds/Door/OpenDoor", transform.position);

        public void PlayCloseDoorSound() =>
            FMODUnity.RuntimeManager.PlayOneShot("event:/Sounds/Door/CloseDoor", transform.position);
    }
}