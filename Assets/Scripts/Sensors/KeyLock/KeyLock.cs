using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Trigger))]
public class KeyLock : MonoBehaviour
{
    private Trigger trigger;
    private Key lockedKey;
    public bool reachedTop{get;private set;}
    private Vector3 topPosition;
    private Quaternion topRotation;
    [HideInInspector] public bool lockedInKey;
    [SerializeField] private float transitionSpeed=5f;
    [SerializeField][Range(1f,2f)] private float sizeTimesThreshold = 1.5f;
    private Vector3 lockedInPosition;
    private Vector3 baseCenterPosition;
    private float baseToCubeRatio = 0.80f;
    private KeyLockGraphics graphics;
    

    /*private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(baseCenterPosition,0.05f);
        Gizmos.DrawCube(transform.position,GetComponentInChildren<MeshRenderer>().bounds.size);
    }*/
    
    private void Awake() {
        MeshRenderer mesh = GetComponentInChildren<MeshRenderer>();
        Quaternion originalRotation = transform.rotation;
        transform.rotation = Quaternion.Euler(0f,0f,0f);
        float baseOffset = mesh.bounds.extents.y;
        transform.rotation = originalRotation;
        lockedInKey = false;
        reachedTop = false;
        trigger = GetComponent<Trigger>();
        Debug.Log(mesh.bounds.extents.y*transform.up);
        baseCenterPosition = transform.position + baseOffset*transform.up;
        //topPosition = baseCenterPosition + 0.85f * mesh.bounds.size.x*transform.up;
        topRotation = Quaternion.Euler(90,0,0)*transform.rotation;
        topPosition = transform.up*GetComponent<SphereCollider>().radius*0.5f + baseCenterPosition +baseToCubeRatio*mesh.bounds.extents.x*transform.up ;
        lockedInPosition = baseCenterPosition + baseToCubeRatio*mesh.bounds.extents.x*transform.up;
        graphics = GetComponentInChildren<KeyLockGraphics>();
    }

    private void Update() {
        if(lockedKey){
            if(!reachedTop){
                lockedKey.transform.position = Vector3.Lerp(lockedKey.transform.position,topPosition,Time.deltaTime*5f);
                lockedKey.transform.rotation = Quaternion.Lerp(lockedKey.transform.rotation,topRotation,Time.deltaTime*5f);
            }
            else{
                if(!lockedInKey){
                    lockedKey.transform.position = Vector3.Lerp(lockedKey.transform.position,lockedInPosition,Time.deltaTime*5f);
                }
            }
            if(!reachedTop && Vector3.Distance(lockedKey.transform.position,topPosition)<0.001f && Quaternion.Angle(lockedKey.transform.rotation,topRotation)<Mathf.Epsilon) reachedTop = true;
            if(reachedTop){
                if(!lockedInKey && Vector3.Distance(lockedKey.transform.position,lockedInPosition)<0.001f){
                    lockedInKey = true;
                    trigger.enter();
                }
            }
        }
    }
    
    private bool checkSize(Key key){
        if(!key) return false;
        else{
            float keySize = key.transform.localScale.x;
            keySize *= key.GetComponentInChildren<KeyGraphics>().transform.localScale.x;
            float ownSize = transform.localScale.x * graphics.transform.localScale.x;
            float sizeTimes = Mathf.Max(ownSize/keySize,keySize/ownSize);
            return sizeTimes <= sizeTimesThreshold;
        }
    }
    private bool checkSizeExact(Key key){
        if(!key) return false;
        else{
            float keySize = key.transform.localScale.x;
            keySize *= key.GetComponentInChildren<KeyGraphics>().transform.localScale.x;
            float ownSize = transform.localScale.x * graphics.transform.localScale.x;
            return Mathf.Approximately(keySize,ownSize);
        }
    }

    private void resizeToFit(Key key){
        if(!key) return;
        Transform keyTransform = key.transform;
        Transform keyGraphics = key.GetComponentInChildren<KeyGraphics>().transform;
        if(keyGraphics.localScale!=Vector3.one*50f){
            keyGraphics.localScale = Vector3.one*50f;
        }
        float ownSize = transform.localScale.x * graphics.transform.localScale.x;
        keyTransform.localScale = Vector3.one * (ownSize / 50f);
    }

    private void OnTriggerEnter(Collider other) {
        if(lockedKey!=null) return;
        if(other.TryGetComponent<Key>(out Key key) && checkSize(key)){
            if(!checkSizeExact(key)){
                resizeToFit(key);
            }
            lockedKey = key;
            key.lockIn(this);
            foreach(Rigidbody rb in lockedKey.GetComponentsInChildren<Rigidbody>()){
                rb.isKinematic = true;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.TryGetComponent<Key>(out Key key) && key == lockedKey && Vector3.Distance(other.transform.position,transform.position)>GetComponent<SphereCollider>().radius*1.3f){
            lockedKey = null;
            lockedInKey = false;
            reachedTop = false;
        }
    }
    public void detach(Key unlockedKey){
        if(unlockedKey == lockedKey){
            foreach(Rigidbody rb in lockedKey.GetComponentsInChildren<Rigidbody>()){
                rb.isKinematic = false;
            }
            graphics.lockOut();
            trigger.leave();
            
        }
    }

    public void graphicLockIn(){
        graphics.lockIn();
    }
}
