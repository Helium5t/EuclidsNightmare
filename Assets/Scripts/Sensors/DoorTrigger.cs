using UnityEngine;

namespace Sensors
{
    [RequireComponent(typeof(AudioSource))]
    public class DoorTrigger : Target
    {
        private Animator animator = null;
        private static readonly int open = Animator.StringToHash("open");

        [SerializeField] public bool startOpen = false;

        private void Start()
        {
            if (!animator && !TryGetComponent<Animator>(out animator)) animator = GetComponentInChildren<Animator>();
            Debug.Log(animator.name);
            SetAnimationBool(startOpen);
        }

        protected override void activate() => SetAnimationBool(true);

        protected override void deactivate() => SetAnimationBool(false);

        private void SetAnimationBool(bool value) => animator.SetBool(open, value);
    }
}