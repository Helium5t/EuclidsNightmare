﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RunnerObject : MonoBehaviour
{
    [SerializeField]private bool animateObject = false;
    [SerializeField][Range(0.1f,30f)] private float rotationSpeed = 2f;
    public Transform targetCheckpoint;

    private Rigidbody ownRigidbody;

    public float runSpeed = 10f;


    public bool isCaught = false;
    // Start is called before the first frame update
    private void OnValidate() {
        if(isCaught){
            isCaught = false;
        }
    }
    private void Awake() {
        if(isCaught){
            isCaught = false;
        }
        ownRigidbody = GetComponent<Rigidbody>();
        ownRigidbody.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(isCaught){
            if(!ownRigidbody.useGravity){
                ownRigidbody.useGravity = true;
            }
            return;
        }
        if(animateObject){
            //transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(0f,transform.rotation.y,0f),Time.deltaTime*5f);
            ownRigidbody.angularVelocity = new Vector3(0f,rotationSpeed,0f);
        }
        if(Vector3.Magnitude(targetCheckpoint.position - transform.position)>=1f){
            ownRigidbody.velocity = Vector3.Normalize(targetCheckpoint.position - transform.position)*runSpeed;
        }
        else{
            ownRigidbody.velocity = targetCheckpoint.position - transform.position;
        }
        //ownRigidbody.velocity  = Vector3.up *2f;
        //transform.position = Vector3.Lerp(transform.position,targetCheckpoint.position,Time.deltaTime);
    }
}
