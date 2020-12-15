using System.Collections;
using UnityEngine;


public abstract class Target : MonoBehaviour
    {
    [SerializeField] public uint require = 1;
    [SerializeField] private float timer_seconds = 0;
    [SerializeField] private bool ignoreTimerIfActive = true;
    [Header("Prefab only fields - do not change outside of prefab editor")]
    [SerializeField] private MeshRenderer debug_mesh = null;
    [SerializeField] private Material debugMaterial = null;

    public bool isActive { get; private set; }
    private uint count = 0;

    private void Awake()
        {
        if (Debug.isDebugBuild)
            {
            debug_mesh.material = new Material(debugMaterial);
            debug_mesh.material.color = Color.red;
            }
        }

    protected abstract void activate();
    public void _activate()
        {
        count++;
        if (count >= require && !isActive)
            {
            if (Debug.isDebugBuild) { debug_mesh.material.color = Color.green; }
            isActive = true;
            if (timer_seconds != 0) { StartCoroutine(deactivateTimer()); }
            activate();
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
        if (Debug.isDebugBuild) { debug_mesh.material.color = Color.red; }
        isActive = false;
        deactivate();
        }

    private IEnumerator deactivateTimer()
        {
        yield return new WaitForSeconds(timer_seconds);
        if (!ignoreTimerIfActive && isActive) { __deactivate(); }
        }
    }