using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]

public class PortalView : MonoBehaviour
{
    [SerializeField] private Transform player;
    private Transform playerCamera;
    [SerializeField] private Transform entryPortal;
    [SerializeField] private Transform exitPortal;
    private float rotationDifference;
    private Vector3 portalOffset;

    
    // Start is called before the first frame update
    void Start()
    {
        if(!entryPortal){
            entryPortal = GameObject.FindGameObjectWithTag("Orange").transform; 
        }
        if(!exitPortal){
            exitPortal = GameObject.FindGameObjectWithTag("Blue").transform;
        }
        rotationDifference =180f+Quaternion.Angle(exitPortal.rotation,entryPortal.rotation);
        playerCamera = player.Find("Camera");
        if(!playerCamera){
            Debug.LogError("No Player attached Camera");
            playerCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        }
        if(!playerCamera){
            Debug.LogError("NO CAMERA FOUND");
        }
        portalOffset = exitPortal.Find("RenderPlaneIn").position -entryPortal.Find("RenderPlaneIn").position ;
    }

    // Update is called once per frame
    void Update()
    {

        gameObject.transform.position = playerCamera.position+portalOffset;
        Vector3 newDir = Quaternion.AngleAxis(rotationDifference,Vector3.up) * playerCamera.forward ;
        gameObject.transform.rotation = Quaternion.LookRotation(newDir);
    }
}
