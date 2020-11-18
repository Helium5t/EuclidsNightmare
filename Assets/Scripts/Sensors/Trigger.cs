using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trigger : MonoBehaviour
    {
    [SerializeField]
    private GameObject targetObject;
    private Target target;

    public bool changeTarget(GameObject targetObject)
        {
        Target target = targetObject.GetComponent<Target>();
        if (target == null) { return false; }

        //this.targetObject = targetObject; //so far it's only useful when editing level from unity, no real advantage in keeping this information at runtime.
        this.target = target;
        return true;
        }

	private void Awake() { changeTarget(targetObject); }

	public void activate() { target.activate(); }
    public void deactivate() { target.deactivate(); }
    
    public abstract void enter();
    public abstract void leave();
    }
