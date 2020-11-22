using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorMovementForces : MonoBehaviour
{
    [SerializeField] private MirrorMovementForces mirror;

    private Vector3 offset;
    private Rigidbody mirrorPhysics;
    private Vector3 previousPos;
    private Quaternion previousRot;

    private bool isMaster = false ;

    [SerializeField][Range(0.0001f,0.9f)] private float stillnessThreshold = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - mirror.transform.position;
        if(mirror.gameObject.TryGetComponent<Rigidbody>(out mirrorPhysics) && !gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            gameObject.AddComponent<Rigidbody>();
        }
        UpdateLastState();
    }

    private void Update() {
        Rigidbody myRb = GetComponent<Rigidbody>();
        if(isMaster){
            Debug.DrawRay(transform.position,Vector3.up*40f,Color.blue);
            Debug.Log(myRb.angularVelocity);
        }
        if(mirror.isMaster){
            if(isMaster){
                isMaster = false;
            }
            myRb.useGravity = false;
            myRb.velocity = mirrorPhysics.velocity;
            transform.rotation = Quaternion.Lerp(transform.rotation,mirror.transform.rotation,Time.deltaTime*5f);
            myRb.angularVelocity = mirrorPhysics.angularVelocity;
            UpdateLastState();
        }
        else{
            if(!IsStill()){
                if(!mirror.isMaster && !isMaster){
                    isMaster = true;
                    myRb.useGravity = true;
                    UpdateLastState();
                }
            }
        }
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if(IsStill()){
            Debug.DrawRay(transform.position,Vector3.forward*10f,Color.red); 
            isMaster = false;
        }
        if(!OffsetIsKept() && isMaster){
            isMaster = false;
            mirror.isMaster = true;
            GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity + (mirror.transform.position+offset - transform.position)*5f;
        }

    }

    // OnValidate is called anytime a value is changed in the unity editor
    void OnValidate () {
        if (mirror != null) {
            mirror.mirror = this;
            mirror.stillnessThreshold = stillnessThreshold;
        }
    }
    
    bool OffsetIsKept(){
        return transform.position == mirror.transform.position + offset;
    }

    bool IsStill(){
        return Vector3.Distance(GetComponent<Rigidbody>().velocity,Vector3.zero)<0.5f && Vector3.Distance(GetComponent<Rigidbody>().angularVelocity,Vector3.zero)<0.5f;
    }
    void UpdateLastState(){
        previousPos = transform.position;
        previousRot = transform.rotation;
    }
}
