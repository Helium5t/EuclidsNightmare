using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleDoor : Executor
    {
    [SerializeField]
    private string id = "A";

    private bool open = false;

	public override void activate() { open = true; }
    public override void deactivate() { open = false; }
    }