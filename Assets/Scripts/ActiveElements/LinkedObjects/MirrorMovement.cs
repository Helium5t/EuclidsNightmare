using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorMovement : MonoBehaviour
{

    [SerializeField] private Transform toFollow;
    private Vector3 offset;
   
    
    // Start is called before the first frame update
    void Start()
    {
        offset = toFollow.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = toFollow.position - offset;
        transform.rotation = toFollow.rotation;
        
    }
}
