using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class PortalPhysicsObject : PortalTraveller {

    public float force = 10;
    new Rigidbody rigidbody;
    static int i;

    protected virtual void Awake () {
        rigidbody = GetComponent<Rigidbody> ();
        if(!graphicsObject){
            graphicsObject = gameObject;
        }
        if(!TryGetComponent<MeshRenderer>(out MeshRenderer renderedmesh)){
            renderedmesh = GetComponentInChildren<MeshRenderer>();
        }
        if(renderedmesh.material.shader != Shader.Find("Custom/Slice")){
            renderedmesh.material = new Material(Shader.Find("Custom/Slice"));
        }
    }

    public override void Teleport (Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot) {
        base.Teleport (fromPortal, toPortal, pos, rot);
        Debug.Log("Teleporting to " + pos );
        rigidbody.velocity = toPortal.TransformVector (fromPortal.InverseTransformVector (rigidbody.velocity));
        rigidbody.angularVelocity = toPortal.TransformVector (fromPortal.InverseTransformVector (rigidbody.angularVelocity));
    }
}