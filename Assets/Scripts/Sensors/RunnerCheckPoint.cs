using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class RunnerCheckPoint : MonoBehaviour
{
    
    public int checkPointNumber=-1;

    private void OnValidate() {
        if(checkPointNumber == -1){
            if(Regex.IsMatch(gameObject.name,"\\d+")){
                checkPointNumber = int.Parse(Regex.Match(gameObject.name,"\\d+").Value);
            }
        }
    }
}
