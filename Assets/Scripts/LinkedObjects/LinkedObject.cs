using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LinkedObject : MonoBehaviour
{
    [SerializeField] private LinkedObject mirror;

    private Vector3 offset;
    private Rigidbody mirrorPhysics;

    private List<Vector3> collisionImpulses;

    private float movementScale=1f;
    private bool isMaster = false ;
    private bool isDragged = false;

    private float masterBid;

    private Vector3 targetOffset;

    [SerializeField][Range(0.0001f,0.9f)] private float stillnessThreshold = 0.5f;
    [SerializeField][Range(0.000f,0.5f)] private float keepAngleThreshold = 0.5f;

    private void Awake() {
        if(!mirror){
            Debug.LogError(gameObject.name + " has no mirror set for it, disabling component.");
            this.enabled = false;
        }
        else{
            targetOffset = transform.position - mirror.transform.position;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        collisionImpulses = new List<Vector3>();
        mirrorPhysics = mirror.gameObject.GetComponent<Rigidbody>();
        masterBid = Random.Range(0f,10f);
    }

    private void Update() {
        if(masterBid == mirror.masterBid){
            masterBid = Random.Range(0f,10f);
        }
        if(isDragged){
            isMaster = true;
            mirror.isMaster = false;
        }
        else{
            if(isMaster){
                isMaster = masterBid > mirror.masterBid;
                mirror.isMaster = mirror.masterBid > masterBid;
            }
        }
    } 
    private void FixedUpdate() {
        Rigidbody myRb = GetComponent<Rigidbody>();
        if(!isMaster){
            if(mirror.isMaster){
                myRb.useGravity = false;
                if(Vector3.Distance(transform.position,mirror.transform.position + targetOffset)>stillnessThreshold){
                    myRb.velocity = (mirror.transform.position + targetOffset - transform.position )*20f;
                }
                else{
                    /*
                    Old script for adding collisions but seems to be okay once moved to fixedUpdate
                    foreach(Vector3 i in collisionImpulses){
                        Debug.Log("Adding Impulse");
                        myRb.AddForce(i,ForceMode.Impulse);
                    }
                    */
                    collisionImpulses = new List<Vector3>();
                    myRb.velocity = myRb.velocity + mirrorPhysics.velocity;
                }
                //transform.rotation = Quaternion.Lerp(transform.rotation,mirror.transform.rotation,Time.deltaTime*10f);
                transform.rotation = mirror.transform.rotation;
            }
            else{
                isMaster = masterBid > mirror.masterBid;
            }
        }
        if(isMaster){
            Debug.DrawRay(transform.position,Vector3.up*5f,Color.blue);
            myRb.useGravity = true;
            collisionImpulses = new List<Vector3>();
            transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(transform.rotation.x,0,transform.rotation.z),Time.deltaTime*10f);
        }
    }

    public void startDrag(){
        isDragged =  true;
    }
    public void stopDrag(){
        isDragged =  false;
    }

    private void OnCollisionEnter(Collision other) {
        if(isMaster){
            mirror.addCollisionImpulse(other.impulse);
        }
    }

    private void addCollisionImpulse(Vector3 impulse){
        if(!isMaster){
            Debug.Log("Added Collision!");
            collisionImpulses.Add(impulse);
        }

    }


    // Update is called once per frame
    /*void LateUpdate()
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

    }*/

    // OnValidate is called anytime a value is changed in the unity editor
    void OnValidate () {
        if (mirror != null) {
            mirror.mirror = this;
            mirror.stillnessThreshold = stillnessThreshold;
            mirror.keepAngleThreshold = keepAngleThreshold;
        }
    }

    public LinkedObject getMirror(){
        return mirror;
    }

    public void scaleReflectedMovement(float newScale){
        movementScale = newScale;
    }

    /*
    bool OffsetIsKept(){
        return transform.position == mirror.transform.position + offset;
    }

    bool IsStill(){
        return Vector3.Distance(GetComponent<Rigidbody>().velocity,Vector3.zero)<0.5f && Vector3.Distance(GetComponent<Rigidbody>().angularVelocity,Vector3.zero)<0.5f;
    }
    void UpdateLastState(){
        previousPos = transform.position;
        previousRot = transform.rotation;
    }*/
}
