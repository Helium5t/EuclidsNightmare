using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private bool useRigidbody = false;
    [SerializeField] private float triggerWeight = 2f;
    [SerializeField] private Transform plateMesh;

    // 0 = x 1 = y 2 = z
    [SerializeField][Range(0,2)] private int movingAxis = 1;
    [SerializeField] private float movingAmount = 0.29f;

    [SerializeField] GameObject triggeredObject;
    private TriggerInterface trigger;

    private Vector3 targetPos;
    private Vector3 startPos;
    private GameObject triggeringObject;
    // Start is called before the first frame update
    void Start()
    {
        if(!plateMesh){
            plateMesh =  gameObject.transform.parent.Find("Plate").transform;
        }
        targetPos= plateMesh.position;
        startPos = plateMesh.position;
        Collider plateCollider = GetComponent<Collider>();
        plateCollider.isTrigger = true;
        trigger = triggeredObject.GetComponent<TriggerInterface>();
    }

    private void Update() {
        if(targetPos != plateMesh.position){
            plateMesh.position = Vector3.Lerp(plateMesh.position, targetPos,Time.deltaTime*2);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(triggeringObject!=null){
            return;
        }
        triggeringObject = other.gameObject;
        if(useRigidbody){
            Rigidbody touchingObject;
            if(other.TryGetComponent<Rigidbody>(out touchingObject)){
                if(touchingObject.mass > triggerWeight){
                    MovePlate();
                    trigger.Trigger();
                }
            }
        }
        else{
            MovePlate();
            trigger.Trigger();
        }
    }

    private void OnTriggerExit(Collider other) {
        if(triggeringObject != null){
            if(other.gameObject == triggeringObject){
                targetPos = startPos;
                triggeringObject = null;
            }
        }
    }

    private void MovePlate(){
        Vector3 moveVector = Vector3.zero;
        if(movingAxis == 0){
            moveVector = new Vector3(movingAmount,0,0);
        }
        if(movingAxis == 1){
            moveVector = new Vector3(0,movingAmount,0);
        }
        if(movingAxis == 2){
            moveVector = new Vector3(0,0,movingAmount);
        }
        targetPos = plateMesh.position  - moveVector;
        
    }
}
