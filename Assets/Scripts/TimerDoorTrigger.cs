using UnityEngine;
using System.Collections;

public class TimerDoorTrigger : MonoBehaviour,TriggerInterface
{
    private Vector3 targetPos;
    private float movementAmount = 10f;

    [SerializeField] private float timeToWait = 10f;
    
    private void Start() {
        targetPos = transform.position;   
    }
    private void Update() {
        if(transform.position != targetPos){
            transform.position = Vector3.Lerp(transform.position,targetPos,Time.deltaTime*2);
        }
    }

    public void Trigger(){
        StartCoroutine(OpenAndClose(timeToWait));
    }

    public IEnumerator OpenAndClose(float timeToWait){
        Vector3 startingPos = transform.position;
        targetPos = transform.position + Vector3.up * movementAmount;
        yield return new WaitForSeconds(timeToWait);
        targetPos = startingPos;
    }

}