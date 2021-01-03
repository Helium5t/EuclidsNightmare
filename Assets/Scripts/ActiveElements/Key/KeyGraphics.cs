using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyGraphics : MonoBehaviour
{
    private Animator animator;
    private readonly int lockedIn = Animator.StringToHash("lockIn");
    private readonly int lockedOut = Animator.StringToHash("lockOut");
    [SerializeField] private Color lockedColor;
    [SerializeField] private Color unlockedColor;
    [SerializeField] private Color currentColor;
    private Color previousColor;
    private MeshRenderer parentRenderer;
    private MeshRenderer[] renderers;


    private void Start() {
        parentRenderer = GetComponentInChildren<MeshRenderer>();
        renderers = GetComponentsInChildren<MeshRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Update() {
        if(currentColor == previousColor) return;
        foreach(MeshRenderer r in renderers){
            foreach(Material m in r.materials){
                if(m.name.Contains("Inner")){
                    m.SetColor("_EmissionColor",currentColor);
                }
            }
        }
        previousColor = currentColor;
    }

    
    public void lockIn(){
        animator.ResetTrigger(lockedOut);
        animator.SetTrigger(lockedIn);
    }

    public void lockOut(){
        Debug.Log("resetting LIN");
        animator.ResetTrigger(lockedIn);
        Debug.Log("Setting LOut");
        animator.SetTrigger(lockedOut);
    }

}
