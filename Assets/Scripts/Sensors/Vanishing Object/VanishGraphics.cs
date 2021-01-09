using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanishGraphics : MonoBehaviour
{
    public bool isVisible = false;
    public float visibility = 0f;
    private MeshRenderer[] renderers;
    // Start is called before the first frame update
    void Awake()
    {
        
        renderers = GetComponentsInChildren<MeshRenderer>();
    }
    public void Initialize(){
        if(!isVisible){
            setVisibility(0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!isVisible && visibility > 0f){
            setVisibility(0f);
        }
    }
    
    public void setVisibility(float visibilityFactor){
        visibility = visibilityFactor;
        foreach(MeshRenderer r in renderers){
            foreach(Material m in r.materials){
                Color c = m.color;
                c.a = visibility;
                m.color = c;
            }
        }
    }
}
