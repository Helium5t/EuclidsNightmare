using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class RunAwayManager : MonoBehaviour
{

    [SerializeField] private Transform chaser;
    [SerializeField] private List<GameObject> checkpoints;
    [SerializeField] private RunnerObject runner;

    [SerializeField][Range(0.0001f,0.2f)] private float keepCheckpointThreshold = 0.05f;

    [SerializeField] List<GameObject> vanishingObjectsOnCatch;
    private bool isRunning = false;

    private float timeElapsed = 0f;
    [SerializeField] private float makeEasierFactor = 1.2f;
    [SerializeField] private float runnerSlowdownFactor = 0.01f;

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
        if(!chaser){
            chaser = GameObject.FindGameObjectWithTag("Player").transform;
        }
        runner.targetCheckpoint = checkpoints[getClosestCheckpoint(runner.transform)].transform;
    }


    // Update is called once per frame
    void Update()
    {
        RunnerCheckPoint currentCheckpoint = runner.targetCheckpoint.gameObject.GetComponent<RunnerCheckPoint>();
        if(!isRunning){
            return;
        }
        if(runner.isCaught){
            isRunning = false;
            foreach(GameObject g in vanishingObjectsOnCatch){
                g.SetActive(false);
            }
            return;
        }
        timeElapsed += Time.deltaTime;
        if(timeElapsed > 10f){
            timeElapsed = 0f;
            FindObjectOfType<FPSController>().expandPickUpDistance(makeEasierFactor);
            runner.runSpeed -= runnerSlowdownFactor;
            runner.runSpeed = Mathf.Max(8.5f,runner.runSpeed);
        }
        if(currentCheckpoint.isReached){
            currentCheckpoint.isReached = false;
            int currentIndex = currentCheckpoint.checkPointNumber - 1;
            //int escapeCheckpoint = getClosestCheckpoint(chaser);
            //runner.targetCheckpoint = getNextCheckpoint(currentIndex,escapeCheckpoint);
            int positiveClosest = getProgressiveClosest(chaser,currentIndex,true);
            int negativeClosest = getProgressiveClosest(chaser,currentIndex,false);
            int positiveDelta = getCheckPointDistance(currentIndex,positiveClosest);
            int negativeDelta = getCheckPointDistance(currentIndex,negativeClosest);
            //Debug.DrawRay(checkpoints[positiveClosest].transform.position,chaser.transform.position - checkpoints[positiveClosest].transform.position ,Color.cyan,0.3f);
            //Debug.DrawRay(checkpoints[negativeClosest].transform.position,chaser.transform.position - checkpoints[negativeClosest].transform.position ,Color.blue,0.3f);
            if(Vector3.Distance(checkpoints[positiveClosest].transform.position,chaser.position) >Vector3.Distance(checkpoints[negativeClosest].transform.position,chaser.position) ){
                runner.targetCheckpoint = checkpoints[(currentIndex +1)%checkpoints.Capacity].transform;
            }
            else{
                if(Vector3.Distance(checkpoints[positiveClosest].transform.position,chaser.position) <Vector3.Distance(checkpoints[negativeClosest].transform.position,chaser.position)){
                    runner.targetCheckpoint = checkpoints[(checkpoints.Capacity + currentIndex - 1)%checkpoints.Capacity].transform;
                }
            }
            
        }
        
    }

    private int getCheckPointDistance(int checkA,int checkB){
        if(checkA>checkB){
            return Mathf.Min(checkA-checkB,checkpoints.Capacity - checkA + checkB);
        }
        else{
            return Mathf.Min(checkB-checkA,checkpoints.Capacity + checkA - checkB);
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

    private int getProgressiveClosest(Transform referenceObject,int referenceCheckpoint,bool positiveDirection){
        float distance = -1f;
        int closestIndex = -1;
        if(positiveDirection){
            for(int i =(referenceCheckpoint +1)%checkpoints.Capacity; i!=referenceCheckpoint;i = (i+1)%checkpoints.Capacity){
                GameObject c = checkpoints[i];
                if(Vector3.Distance(c.transform.position,referenceObject.position) < distance || distance == -1f){
                    distance = Vector3.Distance(c.transform.position,referenceObject.position);
                    closestIndex = i;
                }else{
                    return closestIndex;
                }
            }
        }
        else{
            for(int i = (checkpoints.Capacity + referenceCheckpoint -1)%checkpoints.Capacity; i!=referenceCheckpoint;i = (checkpoints.Capacity + i-1)%checkpoints.Capacity){
                GameObject c = checkpoints[i];
                if(Vector3.Distance(c.transform.position,referenceObject.position) < distance || distance == -1f){
                    distance = Vector3.Distance(c.transform.position,referenceObject.position);
                    closestIndex = i;
                }else{
                    return closestIndex;
                }
            }
        }
        return referenceCheckpoint;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")){
            isRunning = true;
        }
    }

}
