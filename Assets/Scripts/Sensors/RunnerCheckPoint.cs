using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class RunnerCheckPoint : MonoBehaviour
{
    
    public int checkPointNumber=-1;
    [HideInInspector]
    public bool isReached = false;

    private void OnValidate() {
        if(checkPointNumber == -1){
            if(Regex.IsMatch(gameObject.name,"\\d+")){
                checkPointNumber = int.Parse(Regex.Match(gameObject.name,"\\d+").Value);
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.GetComponent<RunnerObject>()){
            isReached = true;
        }
    }
    private void OnTriggerExit(Collider other) {
        if(other.GetComponent<RunnerObject>()){
            isReached = false;
        }
    }

    private void OnTriggerStay(Collider other) {
        if(other.GetComponent<RunnerObject>() && !isReached){
            isReached = true;
        }
    }
}
