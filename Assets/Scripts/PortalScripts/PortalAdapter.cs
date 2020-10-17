using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PortalAdapter : MonoBehaviour
{

    [SerializeField] private Camera portalCamera;
    [SerializeField] private Material portalMaterial;
    // Start is called before the first frame update
    void Start()
    {
        if(portalCamera.targetTexture!=null){
            portalCamera.targetTexture.Release();
        }
        portalCamera.targetTexture = new RenderTexture(Screen.width,Screen.height,24);
        portalMaterial.mainTexture = portalCamera.targetTexture;
        
    }
}
