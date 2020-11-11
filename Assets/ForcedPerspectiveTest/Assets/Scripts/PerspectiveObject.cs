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

    private float startYRotation=0f;
    // Start is called before the first frame update
    void Start()
    {
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
        Debug.DrawRay(pickupRay.origin,pickupRay.direction*200,Color.cyan);
        bool pickupRayIsHitting = Physics.Raycast(pickupRay,out pickupHit,200f,layerMask);
        float startDistance = 0f;
        Vector2 bounds;
        Vector2 boundsIndex;
        bounds = new Vector2(0,0);
        boundsIndex = new Vector2(0,0);
        // Picking object up and setting base values for scaling
        if((Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0)) && pickupRayIsHitting){
            if(pickupHit.transform.tag == "Getable"){
                // Take object
                heldObject = pickupHit.transform.gameObject;

                // Save Distance for multiplication
                startDistance = Vector3.Distance(playerCamera.transform.position,heldObject.transform.position);
                viewAngle = Mathf.Atan(heldObject.GetComponent<MeshRenderer>().bounds.extents.y / startDistance);
                Vector3 scaleMultiplier = heldObject.transform.localScale;
                // if rotation is bad it's because you used transform.eulerangles instead of transform.rotation.eulerangles
                startYRotation = heldObject.transform.eulerAngles.y - playerCamera.transform.eulerAngles.y; 
                bounds = new Vector2(Mathf.Max(heldObject.GetComponent<MeshRenderer>().bounds.size.x,heldObject.GetComponent<MeshRenderer>().bounds.size.y),heldObject.GetComponent<MeshRenderer>().bounds.size.y);
                if(!heldObject.TryGetComponent<Rigidbody>(out Rigidbody rigidbody)){
                    heldObject.AddComponent<Rigidbody>();
                }
                heldObject.GetComponent<Rigidbody>().isKinematic = true;
                if(!heldObject.TryGetComponent<BoxCollider>(out BoxCollider boxCollider)){
                    heldObject.AddComponent<BoxCollider>();
                    heldObject.GetComponent<BoxCollider>().size = new Vector3(bounds.x,bounds.y,bounds.x);
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
            RaycastHit vertUpHit,vertDownHit,leftHit,rightHit,forwardHit;
            bool hitUp,hitDown,hitForward,hitLeft,hitRight;
            bool perspectiveRayIsHitting = Physics.Raycast(pickupRay,out perspectiveHit,200f,layerMask);
            if(heldObject != null){
                BoxCollider sizeBox = heldObject.GetComponent<BoxCollider>();
                Vector3 objectCenter = heldObject.GetComponent<MeshRenderer>().bounds.center;
                hitUp = Physics.Raycast(objectCenter,Vector3.up,out vertUpHit,bounds.y,layerMask);
                hitDown = Physics.Raycast(objectCenter,Vector3.down,out vertDownHit,bounds.y,layerMask);
                Debug.DrawRay(objectCenter,Vector3.up * bounds.y,Color.green);
                Debug.DrawRay(objectCenter,Vector3.down * bounds.y,Color.green);
                Vector3 forwardVec = new Vector3(pickupRay.direction.x,0,pickupRay.direction.z);
                hitForward = Physics.Raycast(objectCenter,forwardVec,out forwardHit,bounds.x,layerMask);
                Vector3 leftVec = Quaternion.AngleAxis(90,Vector3.up)*forwardVec;
                hitLeft = Physics.Raycast(objectCenter,leftVec,out leftHit,bounds.x,layerMask);
                Debug.DrawRay(objectCenter,leftVec,Color.blue);
                Vector3 rightVec = Quaternion.AngleAxis(-90,Vector3.up)*forwardVec;
                hitRight = Physics.Raycast(objectCenter,rightVec,out rightHit,bounds.x,layerMask);
                Debug.DrawRay(objectCenter,rightVec,Color.red);

                if(!(hitUp || hitDown || hitForward || hitLeft || hitRight)){
                    heldObject.transform.position = Vector3.Lerp(heldObject.transform.position,perspectiveHit.point,Time.deltaTime);
                    heldObject.transform.rotation = (Quaternion.AngleAxis(startYRotation+ playerCamera.transform.eulerAngles.y,Vector3.up));
                }
                else{
                    Debug.Log("RayCast Limited");
                }
                if (heldObject.GetComponent<MeshRenderer>() != null)
                {
                    Vector3 centerCorrection = heldObject.transform.position - heldObject.GetComponent<MeshRenderer>().bounds.center;
                }
                
                float scalingFactor = Mathf.Tan(viewAngle) * Vector3.Distance(playerCamera.transform.position,heldObject.transform.position);
                scalingFactor = scalingFactor / heldObject.GetComponent<MeshRenderer>().bounds.extents.y;
                Debug.Log(scalingFactor );
                heldObject.transform.localScale = heldObject.transform.localScale * scalingFactor;
                heldObject.transform.position = perspectiveHit.point - new Vector3(heldObject.GetComponent<MeshRenderer>().bounds.size.x * pickupRay.direction.x,heldObject.GetComponent<MeshRenderer>().bounds.size.y * pickupRay.direction.y,heldObject.GetComponent<MeshRenderer>().bounds.size.z * pickupRay.direction.z);

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
}
