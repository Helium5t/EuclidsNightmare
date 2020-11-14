using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private bool useRigidbody = false;
    [SerializeField] private float triggerWeight = 2f;

    // 0 = x 1 = y 2 = z
    [SerializeField][Range(0,2)] private int movingAxis = 1;
    [SerializeField] private float movingAmount = 0.3f;

    [SerializeField] GameObject triggeredObject;
    private TriggerInterface trigger;
    // Start is called before the first frame update
    void Start()
    {
        Collider plateCollider = GetComponent<Collider>();
        plateCollider.isTrigger = true;
        trigger = triggeredObject.GetComponent<TriggerInterface>();
    }

    private void OnTriggerEnter(Collider other) {
        
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
        transform.position = Vector3.Lerp(transform.position, transform.position - moveVector,Time.deltaTime*2);
    }
}
