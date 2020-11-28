using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunAwayManager : Executor
{
    [SerializeField] private Transform chaser;
    [SerializeField] private List<GameObject> checkpoints;
    [SerializeField] private RunnerObject runner;

    private bool isRunning = false;

    private void Awake() {
        if(checkpoints.Capacity == 0){
            for(int i = 0; i<transform.childCount;i++){
                Transform child = transform.GetChild(i);
                if(child.TryGetComponent<RunnerCheckPoint>(out RunnerCheckPoint checkPoint)){
                    checkpoints.Insert(checkPoint.checkPointNumber,child.gameObject);
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
    }


    // Update is called once per frame
    void Update()
    {
        if(!isRunning  || runner.isCaught){
            return;
        }
        RunnerCheckPoint currentCheckpoint = runner.targetCheckpoint.gameObject.GetComponent<RunnerCheckPoint>();
        int currentIndex = currentCheckpoint.checkPointNumber;
        runner.targetCheckpoint = getNextCheckpoint(currentIndex);
        
    }

    private Transform getNextCheckpoint(int currentCheckpoint){
        Transform prevCheckpoint = checkpoints[currentCheckpoint-1].transform;
        Transform nextCheckpoint = checkpoints[currentCheckpoint+1].transform;
        if(Vector3.Distance(nextCheckpoint.position,chaser.position) >= Vector3.Distance(prevCheckpoint.position,chaser.position)){
            return nextCheckpoint;
        }
        else{
            return prevCheckpoint;
        }
    }

    public override void activate(){
        isRunning = true;
    }
    public override void deactivate(){
        isRunning = false;
    }
}
