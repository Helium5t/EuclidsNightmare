using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggle : Trigger
	{
	bool down = false;

	public override void enter()
		{
		if (down) { deactivate(); }
		else { activate(); }
		down = !down;
		}
	public override void leave() { }
	}