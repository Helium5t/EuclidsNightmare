using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerLooker : MonoBehaviour
{

    private bool mouseLock=false;
    private bool invertLook = false;
    [SerializeField][Range(0.1f,20.0f)] private float horizontalSens=1.0f;
    [SerializeField][Range(0.1f,20.0f)] private float verticalSens = 1.0f;
    [SerializeField] private Transform cameraTransform;

    private Transform character;
    private Quaternion charRotation;
    private Quaternion camRotation;

    // Start is called before the first frame update
    void Start()
    {
        ToggleMouseLock();
        if(!cameraTransform){
            cameraTransform = Camera.main.transform;
        }
        character = gameObject.transform;
        charRotation = character.localRotation;
        camRotation = cameraTransform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        RotateLook();
        if(Input.GetButtonDown("Cancel") && mouseLock){
            ToggleMouseLock();
        }
        if(Input.GetButtonDown("Fire1") && !mouseLock){
            ToggleMouseLock();
        }
        
    }

    void RotateLook(){
        float horizontalRotation = Input.GetAxis("Mouse X") * horizontalSens;
        float verticalRotation = Input.GetAxis("Mouse Y") * verticalSens;

        charRotation *= Quaternion.Euler(0f,horizontalRotation,0f);
        if(invertLook){
            camRotation *= Quaternion.Euler(verticalRotation,0f,0f);
        }
        else{
             camRotation *= Quaternion.Euler(-verticalRotation,0f,0f);
        }
        camRotation = ClampVerticalLook(camRotation);
        
        character.localRotation = charRotation;
        cameraTransform.localRotation = camRotation;
    }

    void ToggleMouseLock(){
        if(!mouseLock){
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            mouseLock = true;
        }
        else{
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            mouseLock = false;
        }
    }

    Quaternion ClampVerticalLook(Quaternion q){
        q.x /= q.w;
		q.y /= q.w;
		q.z /= q.w;
		q.w = 1.0f;
		
		float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);
		
		angleX = Mathf.Clamp (angleX, -90F, 90F);
		
		q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);
		
		return q;
    }
}
