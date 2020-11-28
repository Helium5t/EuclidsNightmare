using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interructor : Trigger
    { 
    public override void firstEnter() { activate(); }
	public override void lastLeave() { deactivate(); }
	}