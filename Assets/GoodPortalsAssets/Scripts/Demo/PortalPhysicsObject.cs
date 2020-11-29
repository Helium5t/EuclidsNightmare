using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class PortalPhysicsObject : PortalTraveller {

    new Rigidbody rigidbody;
    static int i;

    protected virtual void Awake () {
        rigidbody = GetComponent<Rigidbody> ();
        if(!graphicsObject){
            GameObject selfGraphics =  new GameObject(gameObject.name + " (Graphics)");
            selfGraphics.AddComponent<MeshFilter>().sharedMesh = gameObject.GetComponent<MeshFilter>().mesh;
            selfGraphics.AddComponent<MeshRenderer>();
            selfGraphics.GetComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material;
            selfGraphics.transform.localScale = gameObject.transform.localScale;
            selfGraphics.transform.parent = transform;
            selfGraphics.transform.localPosition = Vector3.zero;
            selfGraphics.transform.localRotation = Quaternion.Euler(0f,0f,0f);
            selfGraphics.SetActive(false);
            graphicsObject = selfGraphics;
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