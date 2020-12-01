using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (LinkedObject))]
public class LinkedPortalObject : PortalPhysicsObject {
    enum SpaceState{Compressed,Normal,Expanded}
    
    [SerializeField] private LinkedObject.Direction keepAxis = LinkedObject.Direction.None;

    private float keptAxisOffset = 0f;
    private SpaceState spaceState;

    private LinkedObject objectLink;

    protected override void Awake () {
        base.Awake();
        spaceState = SpaceState.Normal;
        objectLink = GetComponent<LinkedObject>();
    }

    private void Start() {
        if(objectLink.enabled == false){
            Debug.Log(gameObject.name + " linked script is disabled, disabling");
            this.enabled = false;
        }
        if(!objectLink.mirror.TryGetComponent<LinkedPortalObject>(out LinkedPortalObject lpo)){
            Debug.LogError("Mirror has no Linked Portal Object script attached!");
            this.enabled = false;
        }else{
            keptAxisOffset = getAxisOffset();
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
                if(spaceState == SpaceState.Normal){
                        objectLink.resetOffset(keepAxis,keptAxisOffset);
                    }
                else{
                        objectLink.resetOffset();
                }
                objectLink.movementScale = deformedSpace.expandLength(objectLink.movementScale);
                objectLink.mirror.movementScale = deformedSpace.collapseLength(objectLink.mirror.movementScale);
            }
            else{
                if(fromPortal.CompareTag("Compressed Space")){
                    spaceState = deformSpace(SpaceState.Compressed);
                    if(spaceState == SpaceState.Normal){
                        objectLink.resetOffset(keepAxis,keptAxisOffset);
                    }
                    else{
                        objectLink.resetOffset();
                    }
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

    private void OnValidate() 
    {
        objectLink = GetComponent<LinkedObject>();
        if(objectLink!=null &&  !objectLink.mirror.TryGetComponent<LinkedPortalObject>(out LinkedPortalObject lpo)){
            lpo = objectLink.mirror.gameObject.AddComponent<LinkedPortalObject>();
            lpo.keepAxis = this.keepAxis;
            Debug.LogError("Mirror has no linked Portal Object script attached!");
        }
        LinkedPortalObject mirror = objectLink.mirror.GetComponent<LinkedPortalObject>();
        if(mirror.keepAxis != keepAxis && keepAxis!=LinkedObject.Direction.None && mirror.keepAxis!=LinkedObject.Direction.None){
            if(objectLink.masterBid>mirror.objectLink.masterBid){
                mirror.keepAxis = keepAxis;
            }
        }
    }

    private float getAxisOffset(){
        if(keepAxis == LinkedObject.Direction.x){
            return objectLink.targetOffset.x;
        }else if(keepAxis == LinkedObject.Direction.y){
            return objectLink.targetOffset.y;
        }else if(keepAxis == LinkedObject.Direction.z){
            return objectLink.targetOffset.z;
        }else{
            return 0f;
        }
    }
}