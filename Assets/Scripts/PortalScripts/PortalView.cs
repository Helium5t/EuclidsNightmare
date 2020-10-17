using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalView : MonoBehaviour
{
    [SerializeField] private Transform player;
    private Transform playerCamera;
    [SerializeField] private Transform endPortal;
    [SerializeField] private Transform startPortal;
    private float rotationDifference;
    private Vector3 portalOffset;

    
    // Start is called before the first frame update
    void Start()
    {
        if(!endPortal){
            endPortal = GameObject.FindGameObjectWithTag("Orange").transform; 
        }
        if(!startPortal){
            startPortal = GameObject.FindGameObjectWithTag("Blue").transform;
        }
        rotationDifference = 180f + Quaternion.Angle(startPortal.rotation,endPortal.rotation);
        playerCamera = player.Find("Camera");
        if(!playerCamera){
            GameObject.FindGameObjectWithTag("MainCamera");
        }
        portalOffset = startPortal.position - endPortal.position;
    }

    // Update is called once per frame
    void Update()
    {

        gameObject.transform.position = playerCamera.position+portalOffset;
        Vector3 newDir = Quaternion.AngleAxis(rotationDifference,Vector3.up) * playerCamera.forward ;
        gameObject.transform.rotation = Quaternion.LookRotation(newDir);
    }
}
