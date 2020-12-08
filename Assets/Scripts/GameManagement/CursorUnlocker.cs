using UnityEngine;

public class CursorUnlocker : MonoBehaviour
{
    private void Start() => UnlockCursor();

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
