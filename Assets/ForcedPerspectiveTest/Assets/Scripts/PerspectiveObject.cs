using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveObject : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    private GameObject heldObject;
    private Ray pickupRay;

    private float viewAngle = 0f;
    private RaycastHit pickupHit;
    private bool addedCollider = false;
    private LayerMask layerMask = ~(1 << 8);

    private string TODO = "Fix collision and position adjustment on angles of the room. Plus the object sometimes breaks and gets smaller check it out";

    private float startYRotation=0f;
    // Start is called before the first frame update
    void Start()
    {
        Debug.LogError(TODO);
        if(playerCamera == null){
            playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if(Input.GetKeyDown(KeyCode.Mouse0) && Cursor.lockState!=CursorLockMode.Locked){
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        pickupRay = playerCamera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(pickupRay.origin,pickupRay.direction *200f,Color.black);
        bool pickupRayIsHitting = Physics.Raycast(pickupRay,out pickupHit,200f,layerMask);
        float startDistance = 0f;
        Vector2 boundsVec;
        Vector2 boundsIndex;
        boundsVec = new Vector2(0,0);
        boundsIndex = new Vector2(0,0);
        // Picking object up and setting base values for scaling
        if((Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0)) && pickupRayIsHitting){
            if(pickupHit.transform.tag == "Getable"){
                // Take object
                heldObject = pickupHit.transform.gameObject;

                if (heldObject.GetComponent<MeshRenderer>() != null)
                {
                    heldObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    heldObject.GetComponent<MeshRenderer>().receiveShadows = false;
                }
                // Save Distance for multiplication
                startDistance = Vector3.Distance(playerCamera.transform.position,heldObject.transform.position);
                viewAngle = Mathf.Atan(heldObject.GetComponent<MeshRenderer>().bounds.extents.y / startDistance);
                Vector3 scaleMultiplier = heldObject.transform.localScale;
                // if rotation is bad it's because you used transform.eulerangles instead of transform.rotation.eulerangles
                startYRotation = heldObject.transform.eulerAngles.y - playerCamera.transform.eulerAngles.y; 
                boundsVec = new Vector2(Mathf.Max(heldObject.GetComponent<MeshRenderer>().bounds.size.x,heldObject.GetComponent<MeshRenderer>().bounds.size.y),heldObject.GetComponent<MeshRenderer>().bounds.size.y);
                if(!heldObject.TryGetComponent<Rigidbody>(out Rigidbody rigidbody)){
                    heldObject.AddComponent<Rigidbody>();
                }
                heldObject.GetComponent<Rigidbody>().isKinematic = true;
                if(!heldObject.TryGetComponent<BoxCollider>(out BoxCollider boxCollider)){
                    heldObject.AddComponent<BoxCollider>();
                    heldObject.GetComponent<BoxCollider>().size = new Vector3(boundsVec.x,boundsVec.y,boundsVec.x);
                    addedCollider = true;
                }
                foreach(Collider col in heldObject.GetComponents<Collider>()){
                    col.isTrigger = true;
                }
                heldObject.gameObject.layer = 8;
                foreach(Transform child in heldObject.GetComponentsInChildren<Transform>()){
                    child.GetComponent<Rigidbody>().isKinematic = true;
                    child.GetComponent<Collider>().isTrigger = true;
                    child.gameObject.layer = 8;
                }
                if(heldObject.GetComponent<MeshRenderer>().bounds.size.x > heldObject.GetComponent<MeshRenderer>().bounds.size.z){
                    boundsIndex = new Vector2(0,1);
                }
                else{
                    boundsIndex = new Vector2(2,1);
                }
            }
        }
        RaycastHit perspectiveHit;
        // Moving and scaling object
        if((Input.GetKey(KeyCode.E) || Input.GetMouseButton(0)) && pickupRayIsHitting){
            Collider boxReference = heldObject.GetComponent<BoxCollider>();
            //Debug.Log(heldObject.GetComponents<BoxCollider>().GetLength(0));
            RaycastHit upHit,downHit,leftHit,rightHit,forwardHit;
            bool hitUp,hitDown,hitForward,hitLeft,hitRight;
            bool perspectiveRayIsHitting = Physics.Raycast(pickupRay,out perspectiveHit,200f,layerMask);
            if(heldObject != null){
                BoxCollider sizeBox = heldObject.GetComponent<BoxCollider>();
                Vector3 objectCenter = boxReference.bounds.center;
                //Debug.Log(boxReference.bounds);
                Vector3 centerCorrection = new Vector3(0f,0f,0f);
                if (boxReference != null)
                {
                    centerCorrection = heldObject.transform.position - boxReference.bounds.center;
                }
                Debug.Log(centerCorrection);
                
                DrawBoundingBox(boxReference);
                
                float scalingFactor = Mathf.Tan(viewAngle) * Vector3.Distance(playerCamera.transform.position,perspectiveHit.point);
                scalingFactor = scalingFactor / boxReference.bounds.extents.y;
                heldObject.transform.rotation = Quaternion.Lerp(heldObject.transform.rotation,Quaternion.AngleAxis(startYRotation+ playerCamera.transform.eulerAngles.y,Vector3.up),Time.deltaTime*10);
                heldObject.transform.localScale = Vector3.Lerp(heldObject.transform.localScale, heldObject.transform.localScale * scalingFactor,Time.deltaTime * 10);
                heldObject.transform.position = Vector3.Lerp(heldObject.transform.position, perspectiveHit.point + centerCorrection - 0.01f*pickupRay.direction,Time.deltaTime* 10);
                // - new Vector3(boxReference.bounds.size.x * pickupRay.direction.x/2,boxReference.bounds.size.y * pickupRay.direction.y/2,boxReference.bounds.size.z * pickupRay.direction.z/2);
                boundsVec = new Vector2(1f,1f);
                hitUp = Physics.Raycast(objectCenter,Vector3.up,out upHit,100f,layerMask);
                hitDown = Physics.Raycast(objectCenter,Vector3.down,out downHit,100f,layerMask);
                Debug.DrawRay(objectCenter,Vector3.up * 100f,Color.green);
                Debug.DrawRay(objectCenter,Vector3.down * 100f,Color.green);
                Vector3 forwardVec = new Vector3(pickupRay.direction.x,0,pickupRay.direction.z);
                hitForward = Physics.Raycast(objectCenter,forwardVec,out forwardHit,100f,layerMask);
                Vector3 leftVec = Quaternion.AngleAxis(90,Vector3.up)*forwardVec;
                hitLeft = Physics.Raycast(objectCenter,leftVec,out leftHit,100f,layerMask);
                Debug.DrawRay(objectCenter,leftVec*100f,Color.blue);
                Vector3 rightVec = Quaternion.AngleAxis(-90,Vector3.up)*forwardVec;
                hitRight = Physics.Raycast(objectCenter,rightVec,out rightHit,100f,layerMask);
                Debug.DrawRay(objectCenter,rightVec*100f,Color.red);
                //Debug.Log(rightVec*boundsVec.x);
                bool needAdjustment = true;
                int limit = 30;
                if(needAdjustment && limit>0){
                    limit = limit -1;
                    float xAdjustment=0f;
                    float zAdjustment=0f;
                    float yAdjustment=0f;
                    needAdjustment = false;
                    if(hitForward && boxReference.ClosestPoint(forwardHit.point)== forwardHit.point){
                        xAdjustment += forwardVec.x * boundsVec.x;
                        zAdjustment += forwardVec.z * boundsVec.x;
                        needAdjustment = true;
                    }
                    if(hitLeft && boxReference.ClosestPoint(leftHit.point) == leftHit.point){
                        xAdjustment += leftVec.x * boundsVec.x;
                        zAdjustment += leftVec.z * boundsVec.x;
                        needAdjustment = true;
                        
                    }
                    if(hitRight && boxReference.ClosestPoint(rightHit.point) == rightHit.point){
                        xAdjustment += rightVec.x * boundsVec.x;
                        zAdjustment += rightVec.z * boundsVec.x;
                        needAdjustment = true;
                    }
                    if(hitUp && boxReference.ClosestPoint(upHit.point) == upHit.point){
                        yAdjustment += boundsVec.y;
                        needAdjustment = true;
                    }                              
                    if(hitDown && boxReference.ClosestPoint(downHit.point) == downHit.point){
                        yAdjustment += boundsVec.y;
                        needAdjustment = true;
                    }
                    Vector3 adjustmentVector = new Vector3(xAdjustment,yAdjustment,zAdjustment);
                    if(needAdjustment){
                        Debug.Log("Adjusting");
                        heldObject.transform.position = Vector3.Lerp(heldObject.transform.position, perspectiveHit.point + centerCorrection - adjustmentVector,Time.deltaTime* 10);
                        scalingFactor = Mathf.Tan(viewAngle) * Vector3.Distance(playerCamera.transform.position,perspectiveHit.point - adjustmentVector);
                        scalingFactor = scalingFactor / boxReference.bounds.extents.y;
                        heldObject.transform.localScale = Vector3.Lerp(heldObject.transform.localScale, heldObject.transform.localScale * scalingFactor,Time.deltaTime * 10);
                    }
                }
                Debug.Log("Adjusted");
                
                /*
                if(!(hitUp || hitDown || hitForward || hitLeft || hitRight)){
                    heldObject.transform.position = Vector3.Lerp(heldObject.transform.position,perspectiveHit.point,Time.deltaTime);
                    heldObject.transform.rotation = (Quaternion.AngleAxis(startYRotation+ playerCamera.transform.eulerAngles.y,Vector3.up));
                }
                else{
                    Debug.Log("RayCast Limited");
                }*/

            }
        }

        if (Input.GetKeyUp(KeyCode.E) || Input.GetMouseButtonUp(0))
        {
            if (heldObject != null)
            {
                heldObject.GetComponent<Rigidbody>().isKinematic = false;

                foreach (Collider col in heldObject.GetComponents<Collider>())
                {
                    col.isTrigger = false;
                }

                if (heldObject.GetComponent<MeshRenderer>() != null)
                {
                    heldObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                    heldObject.GetComponent<MeshRenderer>().receiveShadows = true;
                }
                heldObject.transform.parent = null;
                heldObject.gameObject.layer = 0;
                foreach (Transform child in heldObject.GetComponentsInChildren<Transform>())
                {
                    heldObject.GetComponent<Rigidbody>().isKinematic = false;
                    heldObject.GetComponent<Collider>().isTrigger = false;
                    child.gameObject.layer = 0;
                }
                if(addedCollider){
                    GameObject.Destroy(heldObject.GetComponent<BoxCollider>());
                    addedCollider = false;
                    
                }

                heldObject = null;
            }
        }



    }
    private void DrawBoundingBox(Collider boxReference){
        Vector3 boxExtents = boxReference.bounds.extents;
        Debug.DrawLine(boxReference.bounds.center + boxExtents,boxReference.bounds.center + new Vector3(boxExtents.x,boxExtents.y,-boxExtents.z),Color.red);
        Debug.DrawLine(boxReference.bounds.center + boxExtents,boxReference.bounds.center + new Vector3(boxExtents.x,-boxExtents.y,boxExtents.z),Color.red);
        Debug.DrawLine(boxReference.bounds.center + boxExtents,boxReference.bounds.center + new Vector3(-boxExtents.x,boxExtents.y,boxExtents.z),Color.red);
        Debug.DrawLine(boxReference.bounds.center + boxExtents*-1f,boxReference.bounds.center + new Vector3(-boxExtents.x,-boxExtents.y,boxExtents.z),Color.red);
        Debug.DrawLine(boxReference.bounds.center + boxExtents*-1f,boxReference.bounds.center + new Vector3(-boxExtents.x,boxExtents.y,-boxExtents.z),Color.red);
        Debug.DrawLine(boxReference.bounds.center + boxExtents*-1f,boxReference.bounds.center + new Vector3(boxExtents.x,-boxExtents.y,-boxExtents.z),Color.red);

        Debug.DrawLine(boxReference.bounds.center + new Vector3(boxExtents.x, boxExtents.y,-boxExtents.z),boxReference.bounds.center + new Vector3(-boxExtents.x,boxExtents.y,-boxExtents.z),Color.red);
        Debug.DrawLine(boxReference.bounds.center + new Vector3(boxExtents.x, boxExtents.y,-boxExtents.z),boxReference.bounds.center + new Vector3(boxExtents.x,-boxExtents.y,-boxExtents.z),Color.red);
        Debug.DrawLine(boxReference.bounds.center + new Vector3(-boxExtents.x,-boxExtents.y,boxExtents.z),boxReference.bounds.center + new Vector3(boxExtents.x,-boxExtents.y,boxExtents.z),Color.red);
        Debug.DrawLine(boxReference.bounds.center + new Vector3(-boxExtents.x,-boxExtents.y,boxExtents.z),boxReference.bounds.center + new Vector3(-boxExtents.x,boxExtents.y,boxExtents.z),Color.red);
    }
}
