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
    private float baseWidth;
    private float baseThickness;
    private bool keyLeaving = false;
    

    /*private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(baseCenterPosition,0.05f);
        Gizmos.DrawCube(transform.position,GetComponentInChildren<MeshRenderer>().bounds.size);
    }*/
    

    private void OnDrawGizmos() {
        Color showed = Color.green;
        showed.a = 0.2f;
        Gizmos.color = showed;
        Gizmos.DrawSphere(transform.position,GetComponent<SphereCollider>().bounds.extents.magnitude);
    }
    private void Awake() {
        MeshRenderer mesh = GetComponentInChildren<MeshRenderer>();
        Quaternion originalRotation = transform.rotation;
        transform.rotation = Quaternion.Euler(0f,0f,0f);
        baseThickness = mesh.bounds.extents.y;
        baseWidth = mesh.bounds.extents.x;
        transform.rotation = originalRotation;
        lockedInKey = false;
        reachedTop = false;
        trigger = GetComponent<Trigger>();
        graphics = GetComponentInChildren<KeyLockGraphics>();
        
        
    }

    private void Start() {
        baseCenterPosition = transform.position + baseThickness*transform.up;
        //topPosition = baseCenterPosition + 0.85f * mesh.bounds.size.x*transform.up;
        topRotation = transform.rotation*Quaternion.Euler(90,0,0);
        topPosition = transform.up*GetComponent<SphereCollider>().bounds.extents.magnitude*baseToCubeRatio + baseCenterPosition - baseToCubeRatio*baseWidth*transform.up ;
        lockedInPosition = baseCenterPosition + baseToCubeRatio*baseWidth*transform.up;
        Debug.DrawLine(baseCenterPosition,topPosition,Color.red,10f);
    }

    private void Update() {
        Debug.DrawLine(baseCenterPosition,topPosition,Color.red,10f);
        if(lockedKey && keyLeaving && Vector3.Distance(lockedKey.transform.position,transform.position)>GetComponent<SphereCollider>().bounds.extents.magnitude){
            releaseKey();
        }
    }

    private float timeFunction(float time){
        float angle = Mathf.Deg2Rad*(180 - (180f*time));
        return (1f + Mathf.Cos(angle)) /2f;
    }

    private IEnumerator lockInKey(Transform keyTransform){
        yield return null;
        Vector3 originalPos = keyTransform.position;
        Quaternion originalRot = keyTransform.rotation;
        float time = 0f;
        #region  move to top
        while(time <= 1f){
            if(!lockedKey) yield break;
            float lerpfactor = timeFunction(time);
            lockedKey.transform.position = Vector3.Lerp(keyTransform.position,topPosition,lerpfactor);
            lockedKey.transform.rotation = Quaternion.Lerp(originalRot,topRotation,lerpfactor);
            if(time < 1f) time = Mathf.Min( time + (Time.deltaTime * transitionSpeed),1f);
            else break;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(0.5f);
        reachedTop = true;
        #endregion
        #region lock in
        time = 0f;
        while(time <= 1f){
            if(!lockedKey) yield break;
            float lerpfactor = timeFunction(time);
            lockedKey.transform.position = Vector3.Lerp(topPosition,lockedInPosition,lerpfactor);
            if(time < 1f) time = Mathf.Min( time + (Time.deltaTime * transitionSpeed),1f);
            else break;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(0.2f);
        lockedInKey = true;
        if(!lockedKey) yield break;
        trigger.enter();
        #endregion
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
        if(lockedKey!=null) {Debug.Log("Still have key"); return;}
        if(other.TryGetComponent<Key>(out Key key) && checkSize(key)){
            Debug.Log("Locking");
            key.lockIn(this);
            if(!checkSizeExact(key)){
                resizeToFit(key);
            }
            lockedKey = key;
            foreach(Rigidbody rb in lockedKey.GetComponentsInChildren<Rigidbody>()){
                rb.isKinematic = true;
            }
            StartCoroutine("lockInKey",other.transform);
        }
    }

    private void OnTriggerExit(Collider other) {
        if( other.TryGetComponent<Key>(out Key key) && key == lockedKey){
            if(Vector3.Distance(other.transform.position,transform.position)>GetComponent<SphereCollider>().radius*1.3f){
                releaseKey();
            }
            else{
                keyLeaving = true;
            }
        }
    }

    private void releaseKey(){
        Debug.Log("Releasing key");
        lockedKey = null;
        lockedInKey = false;
        reachedTop = false;
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
