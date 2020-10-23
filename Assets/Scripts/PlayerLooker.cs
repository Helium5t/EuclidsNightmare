using System;
using UnityEngine;

public class PlayerLooker : MonoBehaviour
{
    [SerializeField] [Range(0.1f, 20.0f)] private float horizontalSens = 1.0f;
    [SerializeField] [Range(0.1f, 20.0f)] private float verticalSens = 1.0f;
    [SerializeField] private Transform cameraTransform;

    private bool mouseLock = false;
    private bool invertLook = false;

    private Transform characterTransform;
    private Quaternion characterRotation;
    private Quaternion cameraRotation;

    private void Awake()
    {
        if (!cameraTransform) cameraTransform = Camera.main.transform;

        characterTransform = GetComponent<Transform>();
        characterRotation = characterTransform.localRotation;
        cameraRotation = cameraTransform.localRotation;
    }

    private void Start()
    {
        ToggleMouseLock();
    }

    private void Update()
    {
        RotateLook();
        if (Input.GetButtonDown("Cancel") && mouseLock) ToggleMouseLock();
        if (Input.GetButtonDown("Fire1") && !mouseLock) ToggleMouseLock();
    }

    // Aligns the camera with the input of the mouse
    private void RotateLook()
    {
        float horizontalRotation = Input.GetAxis("Mouse X") * horizontalSens;
        float verticalRotation = Input.GetAxis("Mouse Y") * verticalSens;

        #region Compute Rotations

        characterRotation *= Quaternion.Euler(0f, horizontalRotation, 0f);

        if (invertLook) cameraRotation *= Quaternion.Euler(verticalRotation, 0f, 0f);
        else cameraRotation *= Quaternion.Euler(-verticalRotation, 0f, 0f);

        #endregion

        cameraRotation = ClampVerticalLook(cameraRotation);

        characterTransform.localRotation = characterRotation;
        cameraTransform.localRotation = cameraRotation;
    }

    // Locks the mouse so the cursor does not escape the window and makes cursor invisible
    private void ToggleMouseLock()
    {
        if (!mouseLock)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            mouseLock = true;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            mouseLock = false;
        }
    }

    private Quaternion ClampVerticalLook(Quaternion q)
    {
        /*
         * Clamp the rotation to a max of +-90 degrees.
         * xi+yj+zk+w AKA ai+bj+ck+d
        */
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, -90f, 90f);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }
}