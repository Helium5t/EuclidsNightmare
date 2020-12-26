using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchFOV : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera parentCamera;
    private Camera ownCamera;
    private float currentFOV;
    void Start()
    {
        parentCamera = transform.parent.GetComponent<Camera>();
        ownCamera = GetComponent<Camera>();
        ownCamera.fieldOfView = parentCamera.fieldOfView;
        currentFOV = ownCamera.fieldOfView;
        ownCamera.depth = parentCamera.depth +1;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentFOV!=parentCamera.fieldOfView){
            ownCamera.fieldOfView = parentCamera.fieldOfView;
        }
    }
}
