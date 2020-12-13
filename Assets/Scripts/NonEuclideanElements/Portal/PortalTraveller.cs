using System.Collections.Generic;
using UnityEngine;

public class PortalTraveller : MonoBehaviour {

    public GameObject graphicsObject;
    public GameObject graphicsClone { get; set; }
    public Vector3 previousOffsetFromPortal { get; set; }

    public Material[] originalMaterials { get; set; }
    public Material[] cloneMaterials { get; set; }

    public virtual void Teleport (Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot) {
        transform.position = pos;
        transform.rotation = rot;
    }

    // Called when first touches portal
    public virtual void EnterPortalThreshold () {
        if (graphicsClone == null) {
            graphicsClone = Instantiate (graphicsObject);
            graphicsClone.transform.parent = graphicsObject.transform.parent;
            graphicsClone.transform.localScale = graphicsObject.transform.localScale;
            originalMaterials = GetMaterials (graphicsObject);
            cloneMaterials = GetMaterials (graphicsClone);
        } else {
            graphicsClone.SetActive (true);
        }
    }

    // Called once no longer touching portal (excluding when teleporting)
    public virtual void ExitPortalThreshold () {
        graphicsClone.SetActive (false);
        // Disable slicing
        for (int i = 0; i < originalMaterials.Length; i++) {
            originalMaterials[i].SetVector ("sliceNormal", Vector3.zero);
        }
    }

    public void SetSliceOffsetDst (float dst, bool clone) {
        for (int i = 0; i < originalMaterials.Length; i++) {
            if (clone) {
                cloneMaterials[i].SetFloat ("sliceOffsetDst", dst);
            } else {
                originalMaterials[i].SetFloat ("sliceOffsetDst", dst);
            }

        }
    }

    protected virtual void Awake(){
        if(!TryGetComponent<MeshRenderer>(out MeshRenderer renderedmesh)){
            renderedmesh = GetComponentInChildren<MeshRenderer>();
        }
        if(renderedmesh.material.shader != Shader.Find("Custom/Slice")){
            renderedmesh.material = new Material(Shader.Find("Custom/Slice"));
        }
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
    }

    Material[] GetMaterials (GameObject g) {
        var renderers = g.GetComponentsInChildren<MeshRenderer> ();
        var matList = new List<Material> ();
        foreach (var renderer in renderers) {
            foreach (var mat in renderer.materials) {
                matList.Add (mat);
            }
        }
        return matList.ToArray ();
    }
}