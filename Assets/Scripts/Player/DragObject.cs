using System;
using UnityEngine;

/*
 * mouseFollowSpeed = speed factor which controls how fast the rigidbody will follow the mouse pointer.
 * maxObjectSpeed = avoid object to pass through thin objects
 */

[RequireComponent(typeof(FPSController))]
[RequireComponent(typeof(AudioSource))]
public class DragObject : MonoBehaviour
{
    [SerializeField] private float mouseFollowSpeed = 5.0f;
    private float maxObjectSpeed = 15.0f;
    private Transform draggedObj;

    private RaycastHit hit;
    private float distanceFromMousePointer;

    private float distanceCorrection =0f;
    [SerializeField][Range(5f,500f)] private float maxPickUpDistance = 10f;
    [SerializeField][Range(0.1f,10f)] private float minHoldingDistance = 3f;

    [SerializeField][Range(0.001f,0.6f)] private float ignoredObstacleMaximumSize = 0.01f;

    // Distance at which the object slows down towards its target (when its close enough to where it should be)
    [SerializeField][Range(0.001f,1f)] private float slowdownThreshold = 0.5f;
    

    private Rigidbody draggedObjectRb;

    private void FixedUpdate() {
        if(draggedObj){
            Ray downwardRay = new Ray(transform.position,Vector3.down);
            if(Physics.Raycast(downwardRay,out RaycastHit downHit,4f) && downHit.rigidbody){
                if(downHit.rigidbody == draggedObjectRb){
                    draggedObjectRb.velocity = draggedObjectRb.velocity + Vector3.down*GetComponent<FPSController>().gravity;
                }
            }
        }
    }
    private void LateUpdate()
    {
        if (Input.GetMouseButton(0) && Time.timeScale!=0f)
        {
            Ray screenPointToRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(screenPointToRay.origin,screenPointToRay.direction * maxPickUpDistance,Color.red);
            if (!draggedObj)
            {
                if (Physics.Raycast(screenPointToRay, out hit,maxPickUpDistance) && hit.rigidbody)
                {
                    if(!hit.transform.TryGetComponent<Portal>(out Portal portal)){
                        draggedObj = hit.transform;
                    }
                    distanceCorrection = ComputeDistanceCorrection(draggedObj,screenPointToRay);
                    distanceFromMousePointer = Mathf.Max(hit.distance + distanceCorrection ,minHoldingDistance);
                    draggedObjectRb = draggedObj.GetComponent<Rigidbody>();

                    notifyDraggedObject();
                    
                }
            }
            else
            {   
                /*
                Vector3 cameraPos = GetComponentInChildren<Camera>().transform.position;
                
                //Breaks dragging
                if(Physics.Raycast(cameraPos,draggedObj.position - cameraPos,out RaycastHit obstacle,distanceFromMousePointer,~LayerMask.GetMask("Portal"))){
                    if(!obstacle.transform.GetComponentInChildren<Portal>() && obstacle.transform != draggedObj){
                        if(obstacle.transform.gameObject.GetComponentInChildren<MeshRenderer>().bounds.size.x > ignoredObstacleMaximumSize){
                            draggedObj = null;
                            return;
                        }
                    }
                }*/
                if(draggedObjectRb.useGravity){
                    draggedObjectRb.useGravity = false;
                }
                Vector3 targetPos  = Vector3.zero;
                if(Physics.Raycast(screenPointToRay,out hit,maxPickUpDistance*1.5f,LayerMask.GetMask("Portal"))){
                    //MeshRenderer portalScreen = hitPortal.transform.Find("Screen").GetComponent<MeshRenderer>();
                    Portal hitPortal = hit.transform.gameObject.GetComponentInParent<Portal>();
                    Vector3 portalRayOrigin = (hit.point - hitPortal.transform.position) + hitPortal.linkedPortal.transform.position;
                    Vector3 portalRayDirection = screenPointToRay.direction;
                    if(hit.distance < distanceFromMousePointer){
                        
                        
                        Ray portalRay = new Ray(portalRayOrigin,portalRayDirection);
                        if(Vector3.Distance(draggedObj.position,hitPortal.transform.position) > Vector3.Distance(draggedObj.position,hitPortal.linkedPortal.transform.position)){
                            targetPos = portalRay.GetPoint(distanceFromMousePointer - hit.distance);
                        }
                        else{
                            targetPos = hit.point + (0.5f - distanceCorrection) *screenPointToRay.direction;
                        }
                    }
                    else if(Vector3.Distance(draggedObj.position,hitPortal.transform.position) > Vector3.Distance(draggedObj.position,hitPortal.linkedPortal.transform.position)){
                        targetPos = portalRayOrigin - (0.5f) * portalRayDirection;
                        Debug.DrawRay(draggedObj.position,portalRayOrigin - draggedObj.position,Color.red);
                    }
                    else{
                        targetPos = screenPointToRay.GetPoint(distanceFromMousePointer);
                    }
                }
                else{
                    targetPos = screenPointToRay.GetPoint(distanceFromMousePointer);
                }
                Debug.DrawRay(draggedObj.position,targetPos- draggedObj.position,Color.green);

                //draggedObj.transform.rotation =  Quaternion.Slerp(draggedObj.transform.rotation,Quaternion.Euler(0f,transform.rotation.y,0f),Time.deltaTime*6f);
                // Calculate needed speed
                draggedObjectRb.angularVelocity = Vector3.zero;
                Quaternion currentRotation = draggedObj.rotation;
                Quaternion targetRotation = new Quaternion(0f,transform.rotation.y,0f,transform.rotation.w);
                Quaternion nextRotation = Quaternion.Lerp(currentRotation,targetRotation,Time.deltaTime*10f);
                draggedObj.transform.rotation = nextRotation;
                //draggedObj.transform.rotation.eulerAngles = Vector3.Lerp(draggedObj.transform.rotation.eulerAngles,Vector3.zero,Time.fixedDeltaTime);
                Vector3 movementToTarget = targetPos - draggedObj.position;
                Vector3 vel;
                if(movementToTarget.sqrMagnitude < Mathf.Pow(slowdownThreshold,2)){
                    vel = movementToTarget;
                }
                else { vel =movementToTarget * mouseFollowSpeed; }
                if (vel.magnitude > maxObjectSpeed) vel = Vector3.Normalize(vel)*maxObjectSpeed;
                draggedObjectRb.velocity = vel;
            }
            
        }
        else
        {
            if(draggedObj){
                if(draggedObj.TryGetComponent<LinkedObject>(out LinkedObject linkedObject)){
                            linkedObject.stopDrag();
                }
                draggedObj = null;
                distanceCorrection = 0f;
                if(draggedObjectRb){
                    draggedObjectRb.useGravity = true;
                    draggedObjectRb = null;
                }
            }
        }
    }

    float ComputeDistanceCorrection(Transform draggedTransform,Ray pickupRay){
        Vector3 rayDirection = pickupRay.direction * -1f;
        float minForward = Mathf.Min(Vector3.Angle(rayDirection,draggedTransform.forward),Vector3.Angle(rayDirection,draggedTransform.up));
        float minRight = Mathf.Min(Vector3.Angle(rayDirection,draggedTransform.right),Vector3.Angle(rayDirection,draggedTransform.right*-1f));
        float minUp = Mathf.Min(Vector3.Angle(rayDirection,draggedTransform.forward),Vector3.Angle(rayDirection,draggedTransform.up));
        if(minForward<minRight){
            if(minForward<minUp){
                return draggedTransform.GetComponentInChildren<MeshRenderer>().bounds.extents.z;
            }
            else{
                //minUp
                return draggedTransform.GetComponentInChildren<MeshRenderer>().bounds.extents.y;
            }
        }
        else{
            if(minRight<minUp){
                //minright
                return draggedTransform.GetComponentInChildren<MeshRenderer>().bounds.extents.x;
            }
            else{
                //minup
                return draggedTransform.GetComponentInChildren<MeshRenderer>().bounds.extents.y;
            }
        }
    }

    void notifyDraggedObject(){
        if(draggedObj.TryGetComponent<LinkedObject>(out LinkedObject linkedObject)){
            linkedObject.startDrag();
        }
        if(draggedObj.TryGetComponent<RunnerObject>(out RunnerObject runner)){
            runner.isCaught = true;
        }
    }

    public void expandPickUpDistance(float factor) => maxPickUpDistance *= factor;
    
}