using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class CheckPoint : MonoBehaviour
{
    private CheckPointManager checkPointManager;
    private void Awake() {
        GetComponent<SphereCollider>().isTrigger = true;
        checkPointManager = GetComponentInParent<CheckPointManager>();
    }
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")){
            checkPointManager.activateCheckpoint(this);
        }
    }
}
