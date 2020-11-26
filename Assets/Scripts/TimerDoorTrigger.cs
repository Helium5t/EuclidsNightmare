using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class TimerDoorTrigger : MonoBehaviour,TriggerInterface
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

    public void Trigger(){
        if(!triggered){
            triggered = true;
            StartCoroutine(OpenAndClose(timeToWait));
        }
        else{
            Debug.Log("I'm already triggered");
        }
    }

    public IEnumerator OpenAndClose(float timeToWait){
        doorAnimator.SetBool("open",true);
        yield return new WaitForSeconds(timeToWait);
        doorAnimator.SetBool("open",false);
        AnimatorClipInfo[] animInfo =  doorAnimator.GetCurrentAnimatorClipInfo(0);
        yield return new WaitForSeconds(animInfo[0].clip.length);
        triggered = false;
        yield return null;
    }

}