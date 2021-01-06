using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float rotationTime =5f;
    [SerializeField] private bool clockwiseRotation = true;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float timePassed = Time.deltaTime;
        float angleStep = 360f * timePassed / rotationTime;
        if(!clockwiseRotation){
            angleStep *=-1;
        }
        transform.eulerAngles = Vector3.up *  ((transform.eulerAngles.y + angleStep)%360f);

    }
}
