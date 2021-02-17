using System.Collections;
using UnityEngine;


public abstract class Target : MonoBehaviour
    {
    [SerializeField] private bool toggle = false;
    [SerializeField] public uint require = 1;
    [SerializeField] private bool hasTimer = false;
    [SerializeField] private float timerSeconds = 0;
    [SerializeField] private bool ignoreTimerIfActive = true;
    [Header("Prefab only fields - do not change outside of prefab editor")]

    [SerializeField] private bool useDebugColors = false;
    [SerializeField] private MeshRenderer debugMesh = null;
    [SerializeField] private Material debugMaterial = null;

    public bool isActive { get; private set; }
    private uint count = 0;

    private void Awake()
        {
        if (Debug.isDebugBuild && debugMaterial!=null)
            {
            debugMesh.material = new Material(debugMaterial);
            debugMesh.material.color = Color.red;
            }
        }

    protected abstract void activate();
    public void _activate()
        {
        count++;
        Debug.Log(count);

        if (count >= require)
            {
            if (!isActive)
                {
                if (Debug.isDebugBuild && useDebugColors) { debugMesh.material.color = Color.green; }
                isActive = true;
                if (hasTimer && timerSeconds != 0) { Debug.Log("timer started"); StartCoroutine(deactivateTimer()); }
                activate();
                }
            else if (toggle)
                {
                if (Debug.isDebugBuild && useDebugColors) { debugMesh.material.color = Color.red; }
                isActive = false;
                deactivate(); 
                }
            } 
            
        }

    protected abstract void deactivate();
    public void _deactivate()
        {
        count--;
        if (count < require && isActive) { __deactivate(); }
        }

    private void __deactivate()
        {
        Debug.Log("timer ended");
        if (!toggle)
            {
            if (Debug.isDebugBuild && useDebugColors) { debugMesh.material.color = Color.red; }
            isActive = false;
            deactivate();
            }
        }

    private IEnumerator deactivateTimer()
        {
        yield return new WaitForSeconds(timerSeconds);
        if (!ignoreTimerIfActive && isActive) { __deactivate(); }
        }
    }