using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class TimerDoorTrigger : Executor
{
    private Animator doorAnimator;

    private float animationSpeed = 5f;
    private float movementAmount = 10f;

    [SerializeField] private float timeToWait = 10f;
    
    public bool triggered = false;

    public bool open = false;

    private void Start() {
        if(!doorAnimator){
            doorAnimator = GetComponent<Animator>();
        }
    }

    public override void deactivate()
    {   
        StartCoroutine(Close());
    }

    public override void activate(){
        if(!triggered){
            triggered = true;
            StartCoroutine(Open(timeToWait));
        }
        else{
            Debug.Log("I'm already triggered");
        }
    }

    public IEnumerator Open(float timeToWait){
        doorAnimator.SetBool("open",true);
        yield return new WaitForSeconds(timeToWait);
        deactivate();
        yield return null;
    }

    public IEnumerator Close(){
        doorAnimator.SetBool("open",false);
        AnimatorClipInfo[] animInfo =  doorAnimator.GetCurrentAnimatorClipInfo(0);
        yield return new WaitForSeconds(animInfo[0].clip.length);
        triggered = false;
        yield return null;
    }

}