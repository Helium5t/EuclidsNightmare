using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interructor : Trigger
    { 
    public override void enter() { activate(); }
	public override void leave() { deactivate(); }
	}