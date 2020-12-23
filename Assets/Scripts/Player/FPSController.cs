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
    
        // Distance at which the object slows down towards its target (when its close enough to where it should be)
        [SerializeField][Range(0.001f,1f)] private float slowdownThreshold = 0.5f;
    

        #region  Internal Dragging Values
        // Internal Dragging values
        private Transform draggedObj;
        private RaycastHit hit;
        private float distanceFromMousePointer;
        private float distanceCorrection =0f;
        private Rigidbody draggedObjectRb;
        private float timeSinceLastSeen;
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

        #region Start Up
        protected override void Awake(){
            base.Awake();
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
            if (Input.GetMouseButton(1)){
                //Debug.Log("CLICK " + velocity);
            }
            if (Input.GetMouseButton(0) && Time.timeScale!=0f)
            {
            
                Ray screenPointToRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                //Debug.DrawRay(screenPointToRay.origin,screenPointToRay.direction * maxPickUpDistance,Color.red);
                if (!draggedObj)
                {
                    startDragging(screenPointToRay);
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
                    checkLineOfSight();
                    dragObject(screenPointToRay);
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
                verticalVelocity = Mathf.Min(verticalVelocity, -maxFallSpeed);
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
        private void startDragging(Ray dragRay){
            if (Physics.Raycast(dragRay, out hit,maxPickUpDistance) && hit.rigidbody)
            {
                PlayPickUpSound();
                if(!hit.transform.TryGetComponent<Portal>(out Portal portal)){
                    draggedObj = hit.transform;
                }
                distanceCorrection = ComputeDistanceCorrection(draggedObj,dragRay);
                distanceFromMousePointer = Mathf.Max(hit.distance + distanceCorrection ,minHoldingDistance);
                draggedObjectRb = draggedObj.GetComponent<Rigidbody>();
                notifyDraggedObject();
                    
            }
        }

        private void PlayPickUpSound()
        {
            FMODUnity.RuntimeManager.PlayOneShot(GameSoundPaths.PickUpEventPath, transform.position);
        }


        private void dragObject(Ray dragRay){
            if(draggedObjectRb.useGravity){
                draggedObjectRb.useGravity = false;
            }
            Vector3 targetPos  = Vector3.zero;
            if(Physics.Raycast(dragRay,out hit,maxPickUpDistance*1.5f,LayerMask.GetMask("Portal"))){
                //MeshRenderer portalScreen = hitPortal.transform.Find("Screen").GetComponent<MeshRenderer>();
                Portal hitPortal = hit.transform.gameObject.GetComponentInParent<Portal>();
                Vector3 portalRayOrigin = (hit.point - hitPortal.transform.position) + hitPortal.linkedPortal.transform.position;
                Vector3 portalRayDirection = dragRay.direction;
                if(hit.distance < distanceFromMousePointer){
                    Debug.LogError("hit distance is shorter than holding distance");
                    Ray portalRay = new Ray(portalRayOrigin,portalRayDirection);
                    if(Vector3.Distance(draggedObj.position,hitPortal.transform.position) > Vector3.Distance(draggedObj.position,hitPortal.linkedPortal.transform.position)){
                        targetPos = portalRay.GetPoint(distanceFromMousePointer - hit.distance);
                    }
                    else{
                        targetPos = hit.point + (0.5f - distanceCorrection) *dragRay.direction;
                    }
                }
                else if(Vector3.Distance(draggedObj.position,hitPortal.transform.position) > Vector3.Distance(draggedObj.position,hitPortal.linkedPortal.transform.position)){
                    targetPos = portalRayOrigin - (0.5f) * portalRayDirection;
                    //Debug.DrawRay(draggedObj.position,portalRayOrigin - draggedObj.position,Color.red);
                }
                else{
                    targetPos = dragRay.GetPoint(distanceFromMousePointer);
                }
            }
            else{
                targetPos = dragRay.GetPoint(distanceFromMousePointer);
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

        void notifyDraggedObject(){
            if(draggedObj.TryGetComponent<LinkedObject>(out LinkedObject linkedObject)){
                linkedObject.startDrag();
            }
            if(draggedObj.TryGetComponent<RunnerObject>(out RunnerObject runner)){
                runner.isCaught = true;
            }
        }

        public void expandPickUpDistance(float factor) => maxPickUpDistance *= factor;
        #endregion
    }
}