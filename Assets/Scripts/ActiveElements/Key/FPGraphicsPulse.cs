using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPGraphicsPulse : MonoBehaviour
{
    [SerializeField][Range(1f,10f)] private float cycleSpeed = 5f;
    [SerializeField][Range(0.001f,1f)] private float colorIntensity = 0.8f;
    private Color currentColor;
    private Color previousColor;
    private Color targetColor;
    private float[] rgb;
    private int channel;
    private MeshRenderer[] renderers;

    private void Awake() {
        channel = 1;
        rgb = new float[3];
        rgb[0] = colorIntensity;
        rgb[1] = 0;
        rgb[2] = 0;
        targetColor = new Color(rgb[0],rgb[1],rgb[2],1); 
        renderers = GetComponentsInChildren<MeshRenderer>();
    }
    private void Update() {
        if(Vector4.Distance(currentColor,targetColor) < 0.04f){
            targetColor = generateNextColor();
        }
        currentColor =  Color.Lerp(currentColor,targetColor,Time.deltaTime*cycleSpeed);
        foreach(MeshRenderer r in renderers){
            foreach(Material m in r.materials){
                if(m.name.Contains("FP")){
                    m.SetColor("_EmissionColor",currentColor);
                }
            }
        }
    }

    private Color generateNextColor(){
        channel = (channel +1)%3;
        rgb[channel] = Mathf.Clamp(rgb[channel],0f,colorIntensity);
        rgb[channel] = (rgb[channel] == colorIntensity)?0f:colorIntensity;
        return new Color(rgb[0],rgb[1],rgb[2],1);
    }
   
}
