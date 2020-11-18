using UnityEngine;
using System.Collections;

public class TimerDoorTrigger : MonoBehaviour,TriggerInterface
{
    private Vector3 targetPos;
    private Vector3 startPos;

    private float animationSpeed = 5f;
    private float movementAmount = 10f;

    [SerializeField] private float timeToWait = 10f;
    
    public bool triggered = false;

    private void Start() {
        startPos = transform.position;
        targetPos = transform.position;   
    }
    private void Update() {
        if(transform.position != targetPos){
            transform.position = Vector3.Lerp(transform.position,targetPos,Time.deltaTime*animationSpeed);
        }
        if(Vector3.Distance(startPos,transform.position)< Mathf.Epsilon && triggered){
            triggered = false;
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
        Vector3 startingPos = transform.position;
        targetPos = transform.position + Vector3.up * movementAmount;
        yield return new WaitForSeconds(timeToWait);
        targetPos = startingPos;
        yield return new WaitForSeconds(Time.deltaTime*animationSpeed);
        triggered = false;
        yield return null;
    }

}