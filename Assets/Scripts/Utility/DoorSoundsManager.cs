﻿using UnityEngine;

namespace Utility
{
    public class DoorSoundsManager : MonoBehaviour
    {
        [SerializeField] private Animator doorAnimator;
        private static readonly int OpenCloseDoorBool = Animator.StringToHash("open");

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.O)) doorAnimator.SetBool(OpenCloseDoorBool, true);
            if (Input.GetKeyDown(KeyCode.C)) doorAnimator.SetBool(OpenCloseDoorBool, false);
        }

        public void PlayOpenDoorSound() =>
            FMODUnity.RuntimeManager.PlayOneShot("event:/Sounds/Door/OpenDoor", transform.position);

        public void PlayCloseDoorSound() =>
            FMODUnity.RuntimeManager.PlayOneShot("event:/Sounds/Door/CloseDoor", transform.position);
    }
}