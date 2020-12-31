using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private CheckPointManager checkPointManager;

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")){
            checkPointManager.activateCheckpoint(this);
        }
    }
}
