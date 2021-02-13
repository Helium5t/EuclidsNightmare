using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class CheckPoint : MonoBehaviour
{
    private CheckPointManager checkPointManager;
    public float deathHeightFromHere = -50f;
    private bool used = false;

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + deathHeightFromHere*Vector3.up,new Vector3(50,0,50));
    }
    private void Awake() {
        GetComponent<SphereCollider>().isTrigger = true;
        checkPointManager = GetComponentInParent<CheckPointManager>();
    }
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player") && !used){
            used = true;
            checkPointManager.activateCheckpoint(this);
        }
    }
}
