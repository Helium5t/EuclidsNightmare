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
                    draggedObj = hit.transform;
                    distanceFromMousePointer = hit.distance;
                    draggedObjectRb = draggedObj.GetComponent<Rigidbody>();
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

                if (vel.magnitude > maxObjectSpeed) vel *= maxObjectSpeed / vel.magnitude;
                draggedObjectRb.velocity = vel;
            }
            
        }
        else
        {
            draggedObj = null;
        }
    }
}