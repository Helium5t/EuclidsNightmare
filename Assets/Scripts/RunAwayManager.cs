using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunAwayManager : MonoBehaviour
{
    [SerializeField] private Transform chaser;
    [SerializeField] private List<GameObject> checkpoints;
    [SerializeField] private RunnerObject runner;

    [SerializeField][Range(0.0001f,0.2f)] private float keepCheckpointThreshold = 0.05f;
    private bool isRunning = false;

    private void Awake() {
        if(checkpoints.Capacity == 0){
            for(int i = 0; i<transform.childCount;i++){
                Transform child = transform.GetChild(i);
                if(child.TryGetComponent<RunnerCheckPoint>(out RunnerCheckPoint checkPoint)){
                    checkpoints.Insert(checkPoint.checkPointNumber-1,child.gameObject);
                }
            }
        }
        foreach(GameObject c in checkpoints){
            if(c.TryGetComponent<MeshRenderer>(out MeshRenderer m)){
                m.enabled = false;
            }
            if(c.TryGetComponent<Collider>(out Collider coll) && !coll.isTrigger){
                coll.isTrigger = true;
            }
        }
        if(!runner){
            runner = GetComponentInChildren<RunnerObject>();
        }
        runner.targetCheckpoint = checkpoints[getClosestCheckpoint(runner.transform)].transform;
    }


    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(checkpoints[getClosestCheckpoint(chaser.transform)].transform.position,chaser.transform.position - checkpoints[getClosestCheckpoint(chaser.transform)].transform.position,Color.green);
        if(!isRunning  || runner.isCaught){
            return;
        }
        RunnerCheckPoint currentCheckpoint = runner.targetCheckpoint.gameObject.GetComponent<RunnerCheckPoint>();
        Debug.DrawRay(currentCheckpoint.transform.position,chaser.transform.position - currentCheckpoint.transform.position ,Color.blue);
        Debug.DrawRay(currentCheckpoint.transform.position,runner.transform.position - currentCheckpoint.transform.position ,Color.red);
        if(currentCheckpoint.isReached){
            currentCheckpoint.isReached = false;
            Debug.Log("Moving Runner");
            int currentIndex = currentCheckpoint.checkPointNumber - 1;
            int escapeCheckpoint = getClosestCheckpoint(chaser);
            runner.targetCheckpoint = getNextCheckpoint(currentIndex,escapeCheckpoint);
        }
        
    }

    private Transform getNextCheckpoint(int currentCheckpoint, int escapeCheckpoint){
        Transform prevCheckpoint = checkpoints[(checkpoints.Capacity + currentCheckpoint-1)%checkpoints.Capacity].transform;
        Transform nextCheckpoint = checkpoints[(currentCheckpoint+1)%checkpoints.Capacity].transform;

        if(escapeCheckpoint > currentCheckpoint){
            if( (checkpoints.Capacity - escapeCheckpoint) + currentCheckpoint > escapeCheckpoint - currentCheckpoint){
                return prevCheckpoint;
            }
            else{
                return nextCheckpoint;
            }
        }
        else{
            if( (checkpoints.Capacity - currentCheckpoint) + escapeCheckpoint > currentCheckpoint - escapeCheckpoint){
                return nextCheckpoint;
            }
            else{
                return prevCheckpoint;
            }
        }
        /*
        my version
        if(Vector3.Distance(nextCheckpoint.position,chaser.position) >= Vector3.Distance(prevCheckpoint.position,chaser.position)){
            return nextCheckpoint;
        }
        else{
            return prevCheckpoint;
        }*/
    }

    private int getClosestCheckpoint(Transform referenceObject){
        float distance = -1f;
        int closestIndex = -1;
        for(int i =0; i<checkpoints.Capacity;i++){
            GameObject c = checkpoints[i];
            if(Vector3.Distance(c.transform.position,referenceObject.position) < distance || distance == -1f){
                distance = Vector3.Distance(c.transform.position,referenceObject.position);
                closestIndex = i;
            }
        }
        return closestIndex;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")){
            isRunning = true;
        }
    }
}
