using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCameraRotation : MonoBehaviour
{
    private Quaternion targetRotation;
    // Start is called before the first frame update
    
    private void Awake() {
        targetRotation = Random.rotation;
    }

    private void Update() {
        if(Quaternion.Angle(transform.rotation,targetRotation)>0.001f){
            transform.rotation = Quaternion.Lerp(transform.rotation,targetRotation,Time.deltaTime/10f);
        }
        else{
            targetRotation = Random.rotation;
        }
    }
}
