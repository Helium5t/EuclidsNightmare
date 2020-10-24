using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PortalAdapter : MonoBehaviour
{

    [SerializeField] List<Camera> cameras;
    [SerializeField] List<Material> materials;

    // Start is called before the first frame update
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
}
