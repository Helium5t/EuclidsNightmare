using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PortalAdapter : MonoBehaviour
{

    [SerializeField] List<Camera> cameras;
    [SerializeField] List<GameObject> portals;

    private void Start() {
        for(int i = 0; i<cameras.Capacity; i++){
            Camera c = cameras[i];
            Transform renderPlaneIn = portals[i].transform.Find("RenderPlaneIn");
            Transform renderPlaneOut = portals[i].transform.Find("RenderPlaneOut");
            Shader portalShader = Shader.Find("Unlit/PortalShader");
            if(portalShader == null){
                Debug.LogError("NO PORTAL SHADER FOUND");
            }
            Material renderMaterial = new Material(portalShader);
            if(c.targetTexture!=null){
                c.targetTexture.Release();
            }
            c.targetTexture = new RenderTexture(Screen.width,Screen.height,24);
            renderMaterial.mainTexture = c.targetTexture;
            renderPlaneIn.GetComponent<Renderer>().material = renderMaterial;
            renderPlaneOut.GetComponent<Renderer>().material = renderMaterial;
        }
    }
    // Start is called before the first frame update
    // This was previously used to load each material into each camera but had issues when integrated into the rest of the project
    /*
    void Start()
    {
        for(int i = 0; i<cameras.Capacity; i++){
            Camera c = cameras[i];
            if(c.targetTexture!=null){
                c.targetTexture.Release();
            }
            c.targetTexture = new RenderTexture(Screen.width,Screen.height,24);
            materials[i].mainTexture = c.targetTexture;
        }
    }
    */
}
