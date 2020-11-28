using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class TimerDoorTrigger : MonoBehaviour, TriggerInterface
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

    public void Trigger()
    {
        if (triggered == false)
        {
            ToggleTriggeredState();
            SetDoorAnimationBool(true);
            StartCoroutine(AnimationHelper());
        }
        else Debug.Log("I'm already triggered");
    }

    private IEnumerator AnimationHelper()
    {
        yield return new WaitForSeconds(timeToWait);
        SetDoorAnimationBool(false);
        yield return null;
    }
}