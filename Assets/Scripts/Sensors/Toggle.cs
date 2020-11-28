using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggle : Trigger
	{
	bool down = false;

	public override void firstEnter()
		{
		if (down) { deactivate(); }
		else { activate(); }
		down = !down;
		}
	public override void lastLeave() { }
	}