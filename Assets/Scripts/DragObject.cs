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

    private Rigidbody draggedObjectRb;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray screenPointToRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (!draggedObj)
            {
                if (Physics.Raycast(screenPointToRay, out hit) && hit.rigidbody)
                {
                    draggedObj = hit.transform;
                    distanceFromMousePointer = hit.distance;
                    draggedObjectRb = draggedObj.GetComponent<Rigidbody>();
                }
            }
            else
            {
                // Calculate needed speed
                Vector3 vel = (screenPointToRay.GetPoint(distanceFromMousePointer) - draggedObj.position) * mouseFollowSpeed;

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