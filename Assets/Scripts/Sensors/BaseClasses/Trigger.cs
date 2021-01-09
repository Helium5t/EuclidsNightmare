using UnityEngine;

public class Trigger : MonoBehaviour
    {
    [SerializeField] private GameObject targetObject = null;
    [SerializeField] private bool toggle = false;
    [Header("Prefab only fields - do not change outside of prefab editor")]
    [SerializeField] private MeshRenderer debugMesh = null;
    [SerializeField] private Material debugMaterial = null;
    [SerializeField] private uint triggerCap = 1;
    private Target target = null;

    public uint entered { get; private set; }
    private bool down = false; //only needed for toggle mode

    public bool isActive { get; private set; }

    public bool changeTarget(GameObject targetObj)
        {
        if (targetObj.TryGetComponent<Target>(out target)) { return true; }
        return false;
        //this.targetObj = targetObj; //so far it's only useful when editing level from unity, no real advantage in keeping this information at runtime.
        //this.target = target;
        //return true;
        }

    private void Awake()
        {
        if (Debug.isDebugBuild && debugMesh != null)
            {
            debugMesh.material = new Material(debugMaterial);
            debugMesh.material.color = Color.red;
            }
        changeTarget(targetObject);
        }
    
    private void Update() {
        if(entered > triggerCap) entered = triggerCap;
    }

    private void activate()
        {
        if (Debug.isDebugBuild && debugMesh != null) { debugMesh.material.color = Color.green; }
        isActive = true;
        target._activate();
        FMODUnity.RuntimeManager.PlayOneShot("event:/Sounds/PressurePlate/PressurePlateNormal",transform.position);
        }

    private void deactivate()
        {
        if (Debug.isDebugBuild && debugMesh != null) { debugMesh.material.color = Color.red; }
        isActive = false;
        target._deactivate();
        }

    private void firstEnter()
        {
        if (toggle)
            {
            if (down) { deactivate(); }
            else { activate(); }
            down = !down;
            }
        else { activate(); }
        }

    private void lastLeave() { if (!toggle) { deactivate(); } }

    public void enter()
        {
            if(!targetObject) return;
        if (entered == 0) { firstEnter(); }
        entered++;
        }

    public void leave()
        {
            if(!targetObject) return;
        Debug.Log("Trigger: left, count: " + entered);
        if (entered == 1) { lastLeave(); }
        if (entered != 0) { entered--; }
        }
    }