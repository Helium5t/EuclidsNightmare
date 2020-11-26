using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (LinkedObject))]
public class LinkedPortalObject : PortalPhysicsObject {
    enum SpaceState{Compressed,Normal,Expanded}

    private SpaceState spaceState;

    private LinkedObject objectLink;

    protected override void Awake () {
        base.Awake();
        spaceState = SpaceState.Normal;
        objectLink = GetComponent<LinkedObject>();
    }

    private void Start() {
        if(!GetComponent<LinkedObject>().enabled == false){
            Debug.Log(gameObject.name + " linked script is disabled, disabling");
            this.enabled = false;
        }
    }


    private void Update() {
    }

// General behaviour
/*
    When entering a non eucldean tunnel (NET), the object entering will change its own movement scale by f and its mirror's by 1/f
    The object that is mirroring (!isMaster) will use its own scale to choose how it should move.
    Example: if object A enters an expanded tunnel (short outside long inside) it will switch to expanded space, its movementScale will be mulitplied by *. 
                object B, instead (mirroring A) will DIVIDE its own movementScale by f, it won't switch to compressed space as it is not the one in the tunnel.
*/
    public override void Teleport (Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot) {
        base.Teleport (fromPortal, toPortal, pos, rot);
        if(fromPortal.GetComponentInParent<NonEuclideanTunnels>()!=null){
            NonEuclideanTunnels deformedSpace = fromPortal.GetComponentInParent<NonEuclideanTunnels>();
            if(fromPortal.CompareTag("Expanded Space")){
                spaceState = deformSpace(SpaceState.Expanded);
                objectLink.resetOffset();
                objectLink.movementScale = deformedSpace.expandLength(objectLink.movementScale);
                objectLink.mirror.movementScale = deformedSpace.collapseLength(objectLink.mirror.movementScale);
            }
            else{
                if(fromPortal.CompareTag("Compressed Space")){
                    spaceState = deformSpace(SpaceState.Compressed);
                    objectLink.resetOffset();
                    objectLink.movementScale = deformedSpace.collapseLength(objectLink.movementScale);
                    objectLink.mirror.movementScale = deformedSpace.expandLength(objectLink.mirror.movementScale);
                }
            }
        }
        else{
            objectLink.resetOffset();
        }
        
        //linkedJoint.maxDistance = Vector3.Distance(linkedJoint.gameObject.transform.position, linkedJoint.connectedBody.transform.position)*1.5f;

    }

    private SpaceState deformSpace(SpaceState spaceDeformation){
        if(spaceState == SpaceState.Compressed){
            if(spaceDeformation == SpaceState.Expanded){
                return SpaceState.Normal;
            }
            else{
                return SpaceState.Compressed;
            }
        }
        if(spaceState == SpaceState.Expanded){
            if(spaceDeformation == SpaceState.Compressed){
                return SpaceState.Normal;
            }
            else{
                return SpaceState.Expanded;
            }
        }
        return spaceDeformation;
    }

    private void OnValidate() {
    }
}