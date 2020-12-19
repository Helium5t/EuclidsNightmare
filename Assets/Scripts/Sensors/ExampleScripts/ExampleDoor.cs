using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleDoor : Target
    {
    [SerializeField]
    private string id = "A";

    private bool open = false;

	protected override void activate() { open = true; }
    protected override void deactivate() { open = false; }
    }