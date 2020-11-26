using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorCamera : MonoBehaviour
{

    enum Orientation {Horizontal,Vertical,Auto};
    [SerializeField] private Camera trackedCamera;
    private Camera mirrorCamera;
    [SerializeField] private Transform mirror;

    [SerializeField] private Orientation orientation = Orientation.Auto;
    [SerializeField] private float nearClipOffset = 0.05f;
    [SerializeField] private float nearClipLimit = 0.2f;

    
    // Start is called before the first frame update
    private void Start() {
        if(!trackedCamera){
            trackedCamera = Camera.main;
        }
        if(!mirror){
            mirror = transform.parent;
        }
        mirrorCamera = GetComponent<Camera>();
        Material mirrorMaterial = new Material(Shader.Find("Unlit/MirrorShader"));
        
        mirrorCamera.targetTexture =  new RenderTexture(Screen.width,Screen.height,24);
        mirrorMaterial.mainTexture = mirrorCamera.targetTexture;
        mirror.GetComponent<MeshRenderer>().material = mirrorMaterial;
        if(orientation == Orientation.Auto){
            if(IsVertical()){
                orientation = Orientation.Vertical;
            }
            else{
                orientation = Orientation.Horizontal;
            }
        }
        if(orientation == Orientation.Horizontal){
            mirrorMaterial.SetInt("_FlippedAxis",0);
        }
        else{
            mirrorMaterial.SetInt("_FlippedAxis",1);
        }

        // extra shit
    }
    

    public void OnPreRender(){
        if (!CameraUtility.VisibleFromCamera (mirror.GetComponent<MeshRenderer>(), trackedCamera) ) {
            return;
        }
        Vector3 objectPos = trackedCamera.transform.position;
        objectPos = mirror.position  -objectPos ;
        objectPos = mirror.position + ReflectV(ReflectV(objectPos,mirror.right),mirror.forward); 
        transform.position = objectPos;
        transform.forward = ReflectV(trackedCamera.transform.forward,mirror.up);
        SetNearClipPlane();
    }

    void SetNearClipPlane () {
        // Learning resource:
        // http://www.terathon.com/lengyel/Lengyel-Oblique.pdf
        Transform clipPlane = mirror.transform;
        int dot = System.Math.Sign (Vector3.Dot (clipPlane.up, mirror.position - transform.position));

        Vector3 camSpacePos = mirrorCamera.worldToCameraMatrix.MultiplyPoint (clipPlane.position);
        Vector3 camSpaceNormal = mirrorCamera.worldToCameraMatrix.MultiplyVector (clipPlane.up) * dot;
        float camSpaceDst = -Vector3.Dot (camSpacePos, camSpaceNormal) + nearClipOffset;

        // Don't use oblique clip plane if very close to portal as it seems this can cause some visual artifacts
        if (Mathf.Abs (camSpaceDst) > nearClipLimit) {
            Vector4 clipPlaneCameraSpace = new Vector4 (camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDst);

            // Update projection based on new clip plane
            // Calculate matrix with player cam so that player camera settings (fov, etc) are used
            mirrorCamera.projectionMatrix = mirrorCamera.CalculateObliqueMatrix (clipPlaneCameraSpace);
        } else {
            mirrorCamera.projectionMatrix = mirrorCamera.projectionMatrix;
        }
    }


    Vector3 getReflectedPos(){
        return Vector3.zero;
    }

    float getSide(Vector3 trackedObject){
        return Mathf.Sign(Vector3.Dot(trackedObject,transform.forward));
    }

    
    Vector3 ReflectV(Vector3 inDirection, Vector3 inNormal)
        {
            float nx = inNormal.x;
            float ny = inNormal.y;
            float nz = inNormal.z;
            float dx = inDirection.x;
            float dy = inDirection.y;
            float dz = inDirection.z;
            float factor = -2F * Vector3.Dot(inNormal, inDirection);
            return new Vector3(factor *nx + dx,
                factor * ny + dy,
                factor * nz + dz);
        }

    bool IsVertical(){
        float closestVerticalAngle = Mathf.Min(Vector3.Angle(Vector3.up,mirror.up),Vector3.Angle(Vector3.down,mirror.up));
        float closestHorizontalAngle = Mathf.Min(Vector3.Angle(Vector3.right,mirror.up),Vector3.Angle(Vector3.left,mirror.up));
        if(closestHorizontalAngle<closestVerticalAngle){
            Debug.Log("Horizontal Smaller");
        }
        Debug.Log(closestHorizontalAngle);
        Debug.Log(closestVerticalAngle);
        return closestVerticalAngle < closestHorizontalAngle;
    }

}
