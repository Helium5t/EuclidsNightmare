using FMOD.Studio;
using GameManagement;
using UnityEngine;
using Utility;
namespace Player
{
    [RequireComponent(typeof(AudioSource))]
    public class FPSController : PortalTraveller
    {
        [Header("Main Settings")] 
        public float walkSpeed = 3;
        public float runSpeed = 6;
        public float smoothMoveTime = 0.1f;

        public bool lockCursor;
        public float mouseSensitivity = 3;
        public Vector2 pitchBounds = new Vector2(-90, 90);
        public float rotationSmoothTime = 0f;

        [Header("Physics Settings")] 
        public float jumpForce = 8;
        public float gravity = 18;
        [SerializeField] private float inputAccelerationFactor = 6f;
        [SerializeField] [Range(0.1f, 5f)] private float maxAirSpeed = 6f;
        [SerializeField] private float airDrag = 0.1f; 
        [SerializeField] [Range(0.1f, 5f)] private float maxFallSpeed = 1.5f;
        CharacterController controller;
        Camera playerCamera;

        [Header("Advanced Settings")] 
        public float rawYaw;
        public float rawPitch;

        [SerializeField] private float jumpFrequency = 0.15f;

        [Header("Object Dragging Settings")]
        [SerializeField] private float mouseFollowSpeed = 5.0f;
        [SerializeField] private float maxObjectSpeed = 15.0f;
        [SerializeField][Range(0f,2f)] private float leniencyTime = 0.8f;
        [SerializeField][Range(5f,500f)] private float maxPickUpDistance = 10f;
        [SerializeField][Range(0.1f,10f)] private float minHoldingDistance = 3f;
        [SerializeField][Range(0.001f,0.6f)] private float ignoredObstacleMaximumSize = 0.01f;
    
        // Distance at which the object sl ows down towards its target (when its close enough to where it should be)
        [SerializeField][Range(0.001f,1f)] private float slowdownThreshold = 0.5f;
    
        [Header("Forced Perspective Settings")]
        [SerializeField] private float maxMarchDistance = 100f;
        [SerializeField][Range(0.001f,10f)] private float marchingStep = 0.1f;
        [SerializeField] private float minMarchDistance = 1f;



        #region  Internal Dragging Values
        // Internal Dragging values
        private Transform draggedObj;
        private RaycastHit hit;
        private float distanceFromMousePointer;
        private float distanceCorrection =0f;
        private Rigidbody draggedObjectRb;
        private float timeSinceLastSeen;
        Ray screenPointToRay;
        float holdingDistance;
#endregion

#region  Internal Values
        // Internal Values
        float yaw;
        float pitch;

        float roll;

        float yawSmoothV;
        float pitchSmoothV;
        float verticalVelocity;
        Vector3 velocity;
        Vector3 smoothV;

        bool jumping;
        float lastGroundedTime;
        bool disabled;
#endregion
#region  Internal NET values
        public bool inTunnel{get; private set;}
#endregion
#region  Internal Forced Perspective Values
        private bool forcedPerspectiveAvailable = false;
        private bool forcedPerspectiveMode = false;
        private MeshRenderer meshBox;
        private Vector3 meshCenterCorrection;
        private float baseSphereRadius = 0f;
        private float startDistance = 0f;
        private Vector3 baseScale;
        private float viewAngle=0f;
        private float startYRotation=0f;
        private float currentMarchedDistance=0f;
        private float currentRadius = 0f;
        private LayerMask ignoreHeldMask;
        private Vector3 targetPerspectivePosition;

#endregion

#region Start Up
        protected override void Awake(){
            base.Awake();
            inTunnel = false;
            ignoreHeldMask = ~LayerMask.GetMask("ForcedPerspectiveHeld","Player");
        }


        private void Start()
        {
       
            playerCamera = Camera.main;
            if (lockCursor)
            {
                cursorLock();
            }

            controller = GetComponent<CharacterController>();

            rawYaw = transform.eulerAngles.y;
            rawPitch = playerCamera.transform.localEulerAngles.x;
            yaw = rawYaw;
            pitch = rawPitch;
            QualitySettings.antiAliasing = 2;
        }
#endregion

#region  Updates
        private void Update(){
            if (PauseMenu.GameIsPaused)
            {
                cursorUnlock();
                disabled = true;
            }
            else if (disabled){
                cursorLock();
                disabled = false;
            }
        #region Debug Pause
            if (Input.GetKeyDown(KeyCode.P))
            {
                cursorUnlock();
                Debug.Break();
            }
        #endregion

            if (disabled) return;


            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            computeMovement(input,isRunning);

            // Jumping Physical Movement
            if (Input.GetKey(KeyCode.Space))
            {
                Jump();
            }

            float mX = Input.GetAxisRaw("Mouse X");
            float mY = Input.GetAxisRaw("Mouse Y");
            rotateView(mX,mY);

        
        }

        private void LateUpdate() {
            #region Debug controls
            /*if (Input.GetMouseButton(1)){
                Debug.Log("CLICK fpm:" + forcedPerspectiveMode);
            }*/
            #endregion
                    
            // forced perspective mode handling
            forcedPerspectiveMode =  (forcedPerspectiveAvailable)? forcedPerspectiveMode:false;
            if (Input.GetKeyDown(KeyCode.F) && forcedPerspectiveAvailable && draggedObj){
                Debug.Log("Switching mode");
                toggleMode();
            }
            else if(Input.GetKeyDown(KeyCode.F)){
                Debug.LogError("Cannot switch.");
                Debug.LogError("Object:" + draggedObj.name);
                Debug.LogError("FPA: "+ forcedPerspectiveAvailable.ToString());

            }
            if (Input.GetMouseButton(0) && Time.timeScale!=0f)
            {
            
                screenPointToRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                //Debug.DrawRay(screenPointToRay.origin,screenPointToRay.direction * maxPickUpDistance,Color.red);
                if (!draggedObj)
                {
                    if (Physics.Raycast(screenPointToRay, out hit,maxPickUpDistance) && hit.rigidbody){
                        if(!hit.transform.TryGetComponent<Portal>(out Portal portal)){
                            draggedObj = hit.transform;
                            forcedPerspectiveAvailable = checkForcedPerspective();
                            startDragging(hit.distance);
                            notifyStartDragObject();
                        }
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
                    if(!forcedPerspectiveMode){
                    checkLineOfSight();
                    dragObject();
                    }
                    else{
                        forcedPerspective();
                    }
                }
            
            }
            else
            {
                if(draggedObj){
                    if(forcedPerspectiveMode){
                        endForcedPerspective();
                    }
                    else{
                        stopDragging();
                    }
                    notifyStopDragObject();
                    draggedObj = null;
                    draggedObjectRb = null;
                    distanceCorrection = 0f;
                    }
            }
        }

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

        #endregion

    #region Non Euclidean
        public override void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
        {
            if(fromPortal.GetComponentInParent<NonEuclideanTunnel>()!= null){
                inTunnel = !inTunnel;
                NonEuclideanTunnel net = fromPortal.GetComponentInParent<NonEuclideanTunnel>();
                net.updateTrackedCamera();
            }
            transform.position = pos;
            Vector3 eulerRot = rot.eulerAngles;
            Vector3 cameraDirection = toPortal.TransformDirection(fromPortal.InverseTransformDirection(playerCamera.transform.forward));
            Quaternion cameraRotation = Quaternion.LookRotation(cameraDirection,toPortal.up);
            playerCamera.transform.localEulerAngles = new Vector3(cameraRotation.eulerAngles.x,0f,cameraRotation.eulerAngles.z);
            float yawDelta = Mathf.DeltaAngle(yaw, eulerRot.y);
            rawYaw =cameraRotation.eulerAngles.y;
            yaw =  cameraRotation.eulerAngles.y;
            rawPitch = (cameraRotation.eulerAngles.x > 180f)? cameraRotation.eulerAngles.x - 360f : cameraRotation.eulerAngles.x;
            pitch = rawPitch;
            roll = cameraRotation.eulerAngles.z;
            transform.eulerAngles = Vector3.up * yaw;
            velocity.y = (controller.isGrounded)? 0f : velocity.y;
            velocity = toPortal.TransformVector(fromPortal.InverseTransformVector(velocity));
            verticalVelocity = velocity.y;
            Physics.SyncTransforms();
            Debug.DrawRay(pos,cameraDirection*20f,Color.red,15f);
            Debug.DrawRay(pos,playerCamera.transform.forward*30f,Color.blue,5f);
        }
    #endregion
    #region Forced Perspective
        private bool checkForcedPerspective(){
            return draggedObj.gameObject.CompareTag("ForcedPerspective");
        }
        private void initForcedPerspective(){
            if(!draggedObj.TryGetComponent<MeshRenderer>(out meshBox)){
                meshBox = draggedObj.GetComponentInChildren<MeshRenderer>();
                if(!meshBox){
                    return;
                }
            }
            meshCenterCorrection = draggedObj.transform.position - meshBox.bounds.center;
            meshBox.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            meshBox.receiveShadows = false;

            // starting measurements for object
            baseSphereRadius = meshBox.bounds.extents.magnitude;
            startDistance = Vector3.Distance(playerCamera.transform.position,draggedObj.transform.position);
            targetPerspectivePosition = screenPointToRay.GetPoint(startDistance);
            draggedObj.transform.position = screenPointToRay.GetPoint(startDistance) + meshCenterCorrection;
            baseScale = draggedObj.transform.localScale;
            viewAngle = Mathf.Atan(baseSphereRadius/startDistance);
            startYRotation = draggedObj.transform.eulerAngles.y - playerCamera.transform.eulerAngles.y;

            // Get Rigidbody and disable collision and physics
            foreach(MeshCollider col in draggedObj.GetComponents<MeshCollider>()){
                if(!col.convex){
                    col.convex = true;
                }
            }
            draggedObjectRb.isKinematic = true;
            foreach(Collider c in draggedObj.GetComponents<Collider>()){
                c.isTrigger = true;
            }

            // Changing object layer
            draggedObj.gameObject.layer = LayerMask.NameToLayer("ForcedPerspectiveHeld");
            foreach(Transform child in draggedObj.GetComponentsInChildren<Transform>()){
                child.GetComponent<Rigidbody>().isKinematic = true;
                child.GetComponent<Collider>().isTrigger = true;
                child.gameObject.layer = LayerMask.NameToLayer("ForcedPerspectiveHeld");
            }
            forcedPerspectiveMode = true;
        }

        private void forcedPerspective(){
            if(Physics.Raycast(screenPointToRay,out RaycastHit destinationHit,maxMarchDistance,ignoreHeldMask)){
                float destinationDistance = destinationHit.distance;
                currentMarchedDistance = minMarchDistance;
                float viewAngleTan = Mathf.Tan(viewAngle);
                float startRadius = viewAngleTan*startDistance;
                currentRadius = viewAngleTan*minMarchDistance;
                while(!Physics.CheckSphere(screenPointToRay.GetPoint(currentMarchedDistance),currentRadius,ignoreHeldMask) && currentMarchedDistance < destinationDistance){
                    currentMarchedDistance += marchingStep;
                    currentRadius = viewAngleTan * currentMarchedDistance;
                }
                #region Debug Prints
                if(currentMarchedDistance > destinationDistance){
                    Debug.Log("Hit Wall at "+ currentMarchedDistance);
                }
                else{
                    Debug.DrawRay(screenPointToRay.GetPoint(currentMarchedDistance),Vector3.up*currentRadius,Color.red);
                    Debug.DrawRay(screenPointToRay.GetPoint(currentMarchedDistance),Vector3.up*-currentRadius,Color.blue);
                }
                #endregion
                float marchedDistance = Mathf.Min(currentMarchedDistance,destinationDistance);
                float distanceCorrection = viewAngleTan*marchedDistance;
                float finalDistance = marchedDistance + distanceCorrection*0f;
                holdingDistance = finalDistance;
                float scalingFactor = currentRadius / startRadius;
                draggedObj.transform.rotation = Quaternion.Lerp(draggedObj.transform.rotation,Quaternion.AngleAxis(startYRotation+ playerCamera.transform.eulerAngles.y,Vector3.up),Time.deltaTime*10);
                if( currentMarchedDistance < destinationDistance && currentMarchedDistance != minMarchDistance){
                    targetPerspectivePosition = screenPointToRay.GetPoint(finalDistance) + (meshCenterCorrection*scalingFactor);
                    draggedObj.transform.localScale = baseScale * scalingFactor ;
                }
                else if(screenPointToRay.GetPoint(minMarchDistance).y < transform.position.y - 1f){
                    Vector3 horizontalDir = new Vector3(screenPointToRay.direction.x,0f,screenPointToRay.direction.z);
                    Vector3 nextPos = new Ray(transform.position,horizontalDir).GetPoint(minMarchDistance) + (meshCenterCorrection*scalingFactor);
                    targetPerspectivePosition.x = nextPos.x;
                    targetPerspectivePosition.z = nextPos.z;
                }
                draggedObj.transform.position = Vector3.Lerp(draggedObj.transform.position, targetPerspectivePosition,Time.deltaTime* 100);
                Debug.DrawLine(playerCamera.transform.position,screenPointToRay.GetPoint(finalDistance),Color.green);
                Debug.DrawLine(playerCamera .transform.position,screenPointToRay.GetPoint(currentMarchedDistance),Color.blue);
            }
        }
        
        private void endForcedPerspective(){
            if(draggedObjectRb){
                draggedObjectRb.isKinematic = false;
            }
            foreach(Collider col in draggedObj.GetComponents<Collider>()){
                col.isTrigger = false;
            }
            if(meshBox){
                meshBox.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                meshBox.receiveShadows = true;
            }
            draggedObj.gameObject.layer = LayerMask.NameToLayer("Default");
            foreach(Transform child in draggedObj.GetComponentsInChildren<Transform>()){
                child.GetComponent<Rigidbody>().isKinematic =false;
                child.GetComponent<Collider>().isTrigger =false;
                child.gameObject.layer = LayerMask.NameToLayer("Default");
            }
            forcedPerspectiveMode = false;
        }
        

        [System.Obsolete("We now use the magnitude of the bounding box")]
        private float getMaxExtent(MeshRenderer mesh){
            return Mathf.Max(mesh.bounds.extents.x,mesh.bounds.extents.y,mesh.bounds.extents.z);
        }

    #endregion
    #region Controls
        private bool correctCameraSwing(float mX,float mY){
            float mMag = Mathf.Sqrt(mX * mX + mY * mY);
            return (mMag > 5);
        }

        private void rotateView(float mX,float mY){
            // Verrrrrry gross hack to stop camera swinging down at start
            if(Time.realtimeSinceStartup<0.5f){
                mX = correctCameraSwing(mX,mY)? 0:mX;
                mY = correctCameraSwing(mX,mY)? 0:mY;
            }
            rawYaw += mX * mouseSensitivity;
            rawPitch -= mY * mouseSensitivity;
            rawPitch = Mathf.Clamp(rawPitch, pitchBounds.x, pitchBounds.y);
            if (rotationSmoothTime > -1f)
            {
                // This should render the movement of the mouse smoother but it gives off a bad feeling during play.
                // It's better to keep this disabled
                pitch = Mathf.SmoothDampAngle(pitch, rawPitch, ref pitchSmoothV, rotationSmoothTime);
                yaw = Mathf.SmoothDampAngle(yaw, rawYaw, ref yawSmoothV, rotationSmoothTime);
            }
            else
            {
                pitch = rawPitch;
                yaw = rawYaw;
            }
            if(playerCamera.transform.localEulerAngles.z != 0f){
                roll = playerCamera.transform.localEulerAngles.z;
            }
            transform.eulerAngles = Vector3.up * yaw;
            playerCamera.transform.localEulerAngles = Vector3.right * pitch;
            if(roll != 0f){
                playerCamera.transform.localEulerAngles = new Vector3(playerCamera.transform.localEulerAngles.x,playerCamera.transform.localEulerAngles.y,Mathf.LerpAngle(roll,0,Time.deltaTime*5f));
            }

        }

        private void cursorLock(){
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void cursorUnlock(){
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    #endregion
    #region Movement
        private void computeMovement(Vector2 movementInput, bool run){

            Vector3 inputDir = new Vector3(movementInput.x, 0, movementInput.y).normalized;
            Vector3 worldInputDir = transform.TransformDirection(inputDir);
            float maxSpeed = maxAirSpeed;
            if(controller.isGrounded){
                maxSpeed = ( (run) ? runSpeed : walkSpeed);
                if(movementInput.x == 0f && movementInput.y == 0f){
                    maxSpeed = 0f;
                }
            }
            Vector3 horizontalAcceleration = worldInputDir * inputAccelerationFactor;
            Vector3 currentHorizontalSpeed = new Vector3(velocity.x,0f,velocity.z);
            Vector3 targetVelocity = Vector3.ClampMagnitude(currentHorizontalSpeed+horizontalAcceleration,maxSpeed);
            velocity = Vector3.SmoothDamp(velocity, targetVelocity, ref smoothV, smoothMoveTime);
            verticalVelocity -= gravity * Time.deltaTime;
            if (verticalVelocity < 0f)
            {
                verticalVelocity = Mathf.Max(verticalVelocity, -maxFallSpeed);
            }
            velocity = new Vector3(velocity.x, verticalVelocity, velocity.z);

            var flags = controller.Move(velocity * Time.deltaTime);
        }

        private void Jump(){
            float timeSinceLastTouchedGround = Time.time - lastGroundedTime;
            if (controller.isGrounded || (!jumping && timeSinceLastTouchedGround < jumpFrequency))
            {
                jumping = true;
                verticalVelocity = jumpForce;
            }
        }
    #endregion
    #region Object Dragging
        private void toggleMode(){
            if(!forcedPerspectiveMode){
                stopDragging();
                initForcedPerspective();
            }
            else{
                endForcedPerspective();
                startDragging(holdingDistance);
            }
        }
        private void checkLineOfSight(){
            if(CameraUtility.VisibleFromCamera(draggedObj.GetComponent<MeshRenderer>(),playerCamera)){
                Vector3 cameraToObject = draggedObj.transform.position - playerCamera.transform.position;
                Ray sightRay = new Ray(playerCamera.transform.position,cameraToObject);
                if(Physics.Raycast(sightRay,out RaycastHit sightHit,cameraToObject.magnitude*1.5f,~LayerMask.GetMask("Trigger"))){
                    if(sightHit.transform != draggedObj){
                        Debug.DrawRay(sightRay.origin,cameraToObject,Color.red);
                    }else{
                        Debug.DrawRay(sightRay.origin,cameraToObject,Color.green);
                    }
                }
            }
        }
        private void startDragging(float dragDist){
            distanceCorrection = ComputeDistanceCorrection(draggedObj,screenPointToRay);
            distanceFromMousePointer = Mathf.Max(dragDist + distanceCorrection ,minHoldingDistance);
            draggedObjectRb = draggedObj.GetComponent<Rigidbody>();
            holdingDistance = dragDist;
        }
        private void stopDragging(){
            if(draggedObjectRb){
                draggedObjectRb.useGravity = true;
            }
        }
        private void PlayPickUpSound()
        {
            FMODUnity.RuntimeManager.PlayOneShot(GameSoundPaths.PickUpEventPath, transform.position);
        }


        private void dragObject(){
            if(forcedPerspectiveMode){
                return;
            }
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
                    Debug.LogError("hit distance is shorter than holding distance");
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
                    //Debug.DrawRay(draggedObj.position,portalRayOrigin - draggedObj.position,Color.red);
                }
                else{
                    targetPos = screenPointToRay.GetPoint(distanceFromMousePointer);
                }
            }
            else{
                targetPos = screenPointToRay.GetPoint(distanceFromMousePointer);
            }
            //Debug.DrawRay(draggedObj.position,targetPos- draggedObj.position,Color.green);

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
        void notifyStartDragObject(){
            if(draggedObj.TryGetComponent<LinkedObject>(out LinkedObject linkedObject)){
                linkedObject.startDrag();
            }
            if(draggedObj.TryGetComponent<RunnerObject>(out RunnerObject runner)){
                runner.isCaught = true;
            }
        }
            
        void notifyStopDragObject(){
            if(draggedObj.TryGetComponent<LinkedObject>(out LinkedObject linkedObject)){
                linkedObject.stopDrag();
            }
        }


        public void expandPickUpDistance(float factor) => maxPickUpDistance *= factor;
    #endregion
    #region Debug Functions
        private void OnDrawGizmos() {
            if(screenPointToRay.direction != Vector3.zero){
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(screenPointToRay.GetPoint(currentMarchedDistance),currentRadius);
            }
        }
    #endregion
    }
}