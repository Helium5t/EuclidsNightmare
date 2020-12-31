using UnityEngine;

namespace Utility
{
    /// <summary>
    /// This script is used only in the Otto Test Scene.
    /// Just for fun!
    /// </summary>
    public class RotateAroundObject : MonoBehaviour
    {
        [SerializeField] private GameObject targetGameObject;
        public int rotationDegreesPerSecond = 20;

        private void Update() => transform.RotateAround(targetGameObject.transform.position, Vector3.up,
            rotationDegreesPerSecond * Time.deltaTime);
    }
}