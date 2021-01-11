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
    [SerializeField] private bool keepLocked = false;
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
        else Debug.Log("Not dragged");
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
            if(keepLocked || !keyLock.lockedInKey){
                player.detachObject();
            }
            else{
                graphicEffects.lockOut();
                keyLock.detach(this);
                lockOut();
                isDragged = true;
            }
        }
        else{
            isDragged = true;
        }
        
    }

    public void stopDragging(){
        isDragged = false;
    }



}
