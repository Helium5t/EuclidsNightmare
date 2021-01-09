using UnityEngine;

namespace Utility
{
    /// <summary>
    /// This class is used only in the MainMenu scene to make sure that the cursor is unlocked whenever it gets loaded.
    /// </summary>
    public class MainMenuCursorUnlock : MonoBehaviour
    {
        private void Start() => CursorUtility.UnlockCursor();
    }
}
