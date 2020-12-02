using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class TimerDoorTrigger : Executor
{
    // private float animationSpeed = 5f;
    // private float movementAmount = 10f;

    [SerializeField] private float timeToWait;

    private Animator doorAnimator;
    private static readonly int Open = Animator.StringToHash("open");

    public bool triggered = false;
    public bool open = false;

    private void Start()
    {
        if (!doorAnimator) doorAnimator = GetComponent<Animator>();
    }

    public void ToggleTriggeredState()
    {
        triggered = !triggered;
        Debug.Log("Triggered state: " + triggered);
    }

    private void SetDoorAnimationBool(bool value) => doorAnimator.SetBool(Open, value);

    public override void activate()
    {
        if (triggered == false)
        {
            ToggleTriggeredState(); //local bool
            SetDoorAnimationBool(true); //sceneTransitionAnimator
            StartCoroutine(AnimationHelper());
        }
        else Debug.Log("I'm already triggered");
    }

    public override void deactivate() => SetDoorAnimationBool(false);

    private IEnumerator AnimationHelper()
    {
        yield return new WaitForSeconds(timeToWait);
        deactivate();
        yield return null;
    }
}