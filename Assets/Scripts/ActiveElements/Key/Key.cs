using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class Key : MonoBehaviour
{
    private bool isLocked = false;
    private KeyGraphics graphicEffects;
    private FPSController player;
    private bool isDragged = false;

    private KeyLock keyLock;

    private void Start() {
        graphicEffects = GetComponentInChildren<KeyGraphics>();
        player = FindObjectOfType<FPSController>();
    }

    private void Update() {
        if(keyLock && keyLock.reachedTop){
            graphicEffects.lockIn();
            keyLock.graphicLockIn();
        }
    }
    public void lockIn(KeyLock locker){
        if(isDragged){
            Debug.Log("Detaching from player");
            player.detachObject();
        }
        isLocked = true;
        keyLock = locker;
    }

    public void lockOut(){
        isLocked =false;
        keyLock = null;
        graphicEffects.lockOut();

    }

    public void startDragging(){
        if(isLocked){
            if(!keyLock.lockedInKey){
                player.detachObject();
            }
            else{
                graphicEffects.lockOut();
                keyLock.detach(this);
                lockOut();
                isDragged = true;
            }
        }
        
    }

    public void stopDragging(){
        isDragged = false;
    }



}
