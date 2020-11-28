using UnityEngine;

[RequireComponent(typeof(Target))]
public abstract class Executor : MonoBehaviour
{
    public abstract void activate();
    public abstract void deactivate();
}