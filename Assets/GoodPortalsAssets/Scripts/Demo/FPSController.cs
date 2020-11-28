using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class FPSController : PortalTraveller
{
    public float walkSpeed = 3;
    public float runSpeed = 6;
    public float smoothMoveTime = 0.1f;
    public float jumpForce = 8;
    public float gravity = 18;

    public bool lockCursor;
    public float mouseSensitivity = 3;
    public Vector2 pitchBounds = new Vector2(-90, 90);
    public float rotationSmoothTime = 0f;

    CharacterController controller;
    Camera playerCamera;
    public float rawYaw;
    public float rawPitch;
    float yaw;
    float pitch;

    float yawSmoothV;
    float pitchSmoothV;
    float verticalVelocity;
    Vector3 velocity;
    Vector3 smoothV;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    bool jumping;
    float lastGroundedTime;
    bool disabled;

    private void Start()
    {
        playerCamera = Camera.main;
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        controller = GetComponent<CharacterController>();

        rawYaw = transform.eulerAngles.y;
        rawPitch = playerCamera.transform.localEulerAngles.x;
        yaw = rawYaw;
        pitch = rawPitch;
    }

    private void Update()
    {
        if (transform.Find("Canvas").Find("PauseMenu").gameObject.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            disabled = true;
        }
        else
        {
            if (disabled)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                disabled = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Break();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            disabled = !disabled;
        }

        if (disabled) return;

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        Vector3 inputDir = new Vector3(input.x, 0, input.y).normalized;
        Vector3 worldInputDir = transform.TransformDirection(inputDir);

        Vector3 targetVelocity = worldInputDir * ((Input.GetKey(KeyCode.LeftShift)) ? runSpeed : walkSpeed);
        velocity = Vector3.SmoothDamp(velocity, targetVelocity, ref smoothV, smoothMoveTime);
        verticalVelocity -= gravity * Time.fixedDeltaTime;
        velocity = new Vector3(velocity.x, verticalVelocity, velocity.z);

        var flags = controller.Move(velocity * Time.deltaTime);
        if (flags == CollisionFlags.Below)
        {
            jumping = false;
            lastGroundedTime = Time.time;
            verticalVelocity = 0;
        }

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space))
        {
            float timeSinceLastTouchedGround = Time.time - lastGroundedTime;
            if (controller.isGrounded || (!jumping && timeSinceLastTouchedGround < 0.15f))
            {
                jumping = true;
                verticalVelocity = jumpForce;
            }
        }

        float mX = Input.GetAxisRaw("Mouse X");
        float mY = Input.GetAxisRaw("Mouse Y");

        // Verrrrrry gross hack to stop camera swinging down at start
        float mMag = Mathf.Sqrt(mX * mX + mY * mY);
        if (mMag > 5)
        {
            mX = 0;
            mY = 0;
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

        transform.eulerAngles = Vector3.up * yaw;
        playerCamera.transform.localEulerAngles = Vector3.right * pitch;
    }

    public override void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        Vector3 eulerRot = rot.eulerAngles;
        float delta = Mathf.DeltaAngle(yaw, eulerRot.y);
        rawYaw += delta;
        yaw += delta;
        transform.eulerAngles = Vector3.up * yaw;
        velocity = toPortal.TransformVector(fromPortal.InverseTransformVector(velocity));
        verticalVelocity = velocity.y;
        Physics.SyncTransforms();
    }
}