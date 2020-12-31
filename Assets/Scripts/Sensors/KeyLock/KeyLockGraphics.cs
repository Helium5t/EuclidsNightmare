using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class KeyLockGraphics : MonoBehaviour
{
    private Animator animator;
    private readonly static int lockInTrigger = Animator.StringToHash("lockIn");
    private readonly static int lockOutTrigger = Animator.StringToHash("lockOut");
    private void Awake() {
        animator = GetComponent<Animator>();
    }

    public void lockIn(){
        animator.ResetTrigger(lockOutTrigger);
        animator.SetTrigger(lockInTrigger);
    }
    public void lockOut(){
        animator.ResetTrigger(lockInTrigger);
        animator.SetTrigger(lockOutTrigger);
    }
}
