using UnityEngine;
using System.Collections;

public class DoorTrigger : Executor
{
    private Animator doorAnimator;
    private static readonly int Open = Animator.StringToHash("open");

    [SerializeField] private bool closeAutomatically = false;

    [SerializeField] private float timeToWait = 10f;

    private bool triggered = false;
    public bool startOpen = false;

    public bool stayOpen = false;

    private void Start()
    {
        if (!doorAnimator)
        {
            doorAnimator = GetComponentInChildren<Animator>();
        }

        SetDoorAnimationBool(startOpen);
    }

    public override void deactivate()
    {
        if (!stayOpen) SetDoorAnimationBool(false);
    }

    public override void activate()
    {
        if (!triggered)
        {
            ToggleTriggeredState();
            SetDoorAnimationBool(triggered);
            if(closeAutomatically){
                StartCoroutine(AnimationHelper());
            }
        }
        else
        {
            Debug.Log("I'm already triggered");
        }
    }

    private IEnumerator AnimationHelper()
    {
        yield return new WaitForSeconds(timeToWait);
        deactivate();
        yield return null;
    }

    public void ToggleTriggeredState()
    {
        triggered = !triggered;
        Debug.Log("Triggered state: " + triggered);
    }

    private void SetDoorAnimationBool(bool value) => doorAnimator.SetBool(Open, value);

}