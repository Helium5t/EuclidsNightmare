using UnityEngine;
using System.Collections;

public class DoorTrigger : Executor
{
    private Animator doorAnimator;

    [SerializeField] private bool closeAutomatically = false;

    private float animationSpeed = 5f;
    private float movementAmount = 10f;

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

        doorAnimator.SetBool("open", startOpen);
    }

    public override void deactivate()
    {
        if (!stayOpen) StartCoroutine(Close());
    }

    public override void activate()
    {
        if (!triggered)
        {
            triggered = true;
            StartCoroutine(Open(timeToWait));
        }
        else
        {
            Debug.Log("I'm already triggered");
        }
    }

    public IEnumerator Open(float timeToWait)
    {
        doorAnimator.SetBool("open", true);
        if (closeAutomatically)
        {
            yield return new WaitForSeconds(timeToWait);
            deactivate();
            yield return null;
        }
        else
        {
            yield return null;
        }
    }

    public IEnumerator Close()
    {
        doorAnimator.SetBool("open", false);
        AnimatorClipInfo[] animInfo = doorAnimator.GetCurrentAnimatorClipInfo(0);
        yield return new WaitForSeconds(animInfo[0].clip.length);
        triggered = false;
        yield return null;
    }
}