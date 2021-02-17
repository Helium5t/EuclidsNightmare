using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotator : MonoBehaviour
{
    [SerializeField] private bool launchScript = false;

    private void OnValidate() {
        if(launchScript){
            rotateRandom();
            launchScript = false;
        }
    }

    private void Start() {
        rotateRandom();
    }

    private void rotateRandom(){
        float amount =  (int) (Random.value * 4f ) * 90f;
        Vector3 rotatedVec = transform.forward * amount;
        transform.localRotation = Quaternion.Euler(rotatedVec.x,rotatedVec.y,rotatedVec.z) * transform.localRotation;
    }
}
