using UnityEngine;

public class DoorControllerTrigger : MonoBehaviour,TriggerInterface
{
    private Vector3 targetPos ;

    private void Start() {
        targetPos = transform.position;   
    }
    private void Update() {
        transform.position = Vector3.Lerp(transform.position,targetPos,Time.deltaTime*2);
    }

    public void Trigger(){
        targetPos = transform.position + Vector3.up * 10f;
    }

}