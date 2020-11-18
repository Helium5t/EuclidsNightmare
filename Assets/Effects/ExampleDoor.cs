using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleDoor : MonoBehaviour, Target
    {

    private bool open = false;

    public void activate()
        {
        open = true;
        Debug.Log("Door is now " + (open ? "open" : "closed") + ".");
        }

    public void deactivate()
        {
        open = false;
        Debug.Log("Door is now " + (open ? "open" : "closed") + ".");
        }
    }