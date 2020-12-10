using UnityEngine;

public abstract class Trigger : MonoBehaviour
{
    [SerializeField] private GameObject targetObject = null;
    private Target target = null;

    public bool changeTarget(GameObject targetObj)
    {
        if (targetObj.TryGetComponent<Target>(out target))
        {
            return true;
        }

        return false;
        //this.targetObj = targetObj; //so far it's only useful when editing level from unity, no real advantage in keeping this information at runtime.
        //this.target = target;
        //return true;
    }

    private void Awake() => changeTarget(targetObject);

    public void activate() => target.activate();

    public void deactivate() => target.deactivate();

    public abstract void firstEnter();
    public abstract void lastLeave();

    private uint entered = 0;

    public void enter()
    {
        if (entered == 0) firstEnter();

        entered++;
    }

    public void leave()
    {
        if (entered == 1) lastLeave();

        entered--;
    }
}