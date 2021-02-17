using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateRandomAnim : MonoBehaviour
{
    [SerializeField] private float rotSpeed = 2f;
    float rotTime = 0f;
    Quaternion startRot;
    Quaternion targetRotation;


    private void Start() {
        startRot = transform.rotation;
        targetRotation = Random.rotation;
    }

    private void Update() {
        rotTime += Time.deltaTime * rotSpeed;
        transform.rotation = Quaternion.Lerp(startRot,targetRotation,rotTime);
        if(rotTime >= 1f){
            rotTime = 0f;
            targetRotation = Random.rotation;
            startRot = transform.rotation;
        }
    }
}
