using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Executor))]
public class Target : MonoBehaviour
	{
	[SerializeField]
	public uint require = 1;

	private Executor executor = null;
	private void Awake()
		{
		//gameObject.GetComponent<Renderer>().material.color = Color.red;
		executor = GetComponent<Executor>();
		}

	private bool on = false;
	private uint count = 0;

	public void activate()
		{
		count++;
		if (count >= require && !on)
			{
			on = true;
			//gameObject.GetComponent<Renderer>().material.color = Color.green;
			executor.activate();
			}
		}
	public void deactivate()
		{
		count--;
		if (count < require && on) 
			{
			on = false;
			//gameObject.GetComponent<Renderer>().material.color = Color.red;
			executor.deactivate();
			}
		}
	}