using UnityEngine;
using Utility;

namespace GameManagement
{
    public class CursorUnlocker : MonoBehaviour
    {
        private void Start() => CursorUtility.UnlockCursor();
    }
}
