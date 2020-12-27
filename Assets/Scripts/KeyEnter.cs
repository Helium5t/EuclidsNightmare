using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyEnter : MonoBehaviour
{
    public Trigger trigger;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        trigger.enter();
    }
    private void OnCollisionExit(Collision collision)
    {
        trigger.leave();
    }
}
