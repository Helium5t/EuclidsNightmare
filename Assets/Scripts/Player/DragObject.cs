using UnityEngine;

/*
 * mouseFollowSpeed = speed factor which controls how fast the rigidbody will follow the mouse pointer.
 * maxObjectSpeed = avoid object to pass through thin objects
 */
public class DragObject : MonoBehaviour
{
    [SerializeField] private float mouseFollowSpeed = 5.0f;
    private float maxObjectSpeed = 15.0f;
    private Transform draggedObj;

    private RaycastHit hit;
    private float distanceFromMousePointer;
    [SerializeField][Range(20f,500f)] private float maxPickUpDistance = 30f;

    private Rigidbody draggedObjectRb;

    private void LateUpdate()
    {
        if (Input.GetMouseButton(0))
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
                    distanceFromMousePointer = hit.distance + DistanceCorrection(draggedObj,screenPointToRay);
                    draggedObjectRb = draggedObj.GetComponent<Rigidbody>();
                    notifyDraggedObject();
                    
                }
            }
            else
            {   
                Vector3 targetPos  = Vector3.zero;
                if(Physics.Raycast(screenPointToRay,out hit,maxPickUpDistance,LayerMask.GetMask("Portal"))){
                    //MeshRenderer portalScreen = hitPortal.transform.Find("Screen").GetComponent<MeshRenderer>();
                    Portal hitPortal = hit.transform.gameObject.GetComponentInParent<Portal>();
                    if(hit.distance < distanceFromMousePointer){
                        Vector3 portalRayOrigin = (hit.point - hitPortal.transform.position) + hitPortal.linkedPortal.transform.position;
                        Vector3 portalRayDirection = screenPointToRay.direction;
                        Debug.DrawRay(portalRayOrigin,portalRayDirection*50f,Color.green);
                        Ray portalRay = new Ray(portalRayOrigin,portalRayDirection);
                        if(Vector3.Distance(draggedObj.position,hitPortal.transform.position) > Vector3.Distance(draggedObj.position,hitPortal.linkedPortal.transform.position)){
                            targetPos = portalRay.GetPoint(distanceFromMousePointer - hit.distance);
                        }
                        else{
                            targetPos = hit.point + 0.2f*screenPointToRay.direction;
                        }
                    }
                    else{
                        targetPos = screenPointToRay.GetPoint(distanceFromMousePointer);
                    }
                }
                else{
                    targetPos = screenPointToRay.GetPoint(distanceFromMousePointer);
                }
                // Calculate needed speed
                Vector3 vel = ( targetPos - draggedObj.position) * mouseFollowSpeed;
                draggedObj.GetComponentInChildren<MeshRenderer>().transform.rotation =  Quaternion.Lerp(draggedObj.GetComponentInChildren<MeshRenderer>().transform.rotation,Quaternion.Euler(0f,0f,0f),Time.deltaTime*10f);
                if (vel.magnitude > maxObjectSpeed) vel *= maxObjectSpeed / vel.magnitude;
                draggedObjectRb.velocity = vel;
            }
            
        }
        else
        {
            if(draggedObj!= null && draggedObj.TryGetComponent<LinkedObject>(out LinkedObject linkedObject)){
                        linkedObject.stopDrag();
            }
            draggedObj = null;
        }
    }

    float DistanceCorrection(Transform draggedTransform,Ray pickupRay){
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
}