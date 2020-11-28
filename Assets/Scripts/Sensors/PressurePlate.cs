using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sensors
{
    public class PressurePlate : MonoBehaviour
    {
        [SerializeField] private bool useRigidbody = false;
        [SerializeField] private float triggerWeight = 2f;
        [SerializeField] private Transform plateMesh;

        // 0 = x 1 = y 2 = z
        [SerializeField] [Range(0, 2)] private int movingAxis = 1;
        [SerializeField] private float movingAmount = 0.29f;

        [SerializeField] GameObject triggeredObject;
        private TriggerInterface trigger;

        private Vector3 targetPos;
        private Vector3 startPos;
        private GameObject triggeringObject;

        [HideInInspector] public bool isTriggered;

        private void Start()
        {
            if (!plateMesh) plateMesh = gameObject.transform.parent.Find("Plate").transform;

            targetPos = startPos = plateMesh.position;
            Collider plateCollider = GetComponent<Collider>();
            plateCollider.isTrigger = true;

            trigger = triggeredObject.GetComponent<TriggerInterface>();
        }

        private void Update()
        {
            if (targetPos != plateMesh.position)
            {
                plateMesh.position = Vector3.Lerp(plateMesh.position, targetPos, Time.deltaTime * 2);
            }
        }

        /*
         * Understand if I have to Animate the pressure plate or not.
         */
        private void OnTriggerEnter(Collider other)
        {
            if (SceneManager.GetActiveScene().name == "GrowingApart")
            {
                GrowingApartLevelLogic(other);
            }
            else
            {
                DefaultLevelLogic(other);
            }
        }

        private void DefaultLevelLogic(Collider other)
        {
            if (triggeringObject != null) return;

            triggeringObject = other.gameObject;
            if (useRigidbody)
            {
                if (other.TryGetComponent<Rigidbody>(out var touchingObject))
                {
                    if (touchingObject.mass > triggerWeight)
                    {
                        isTriggered = true;
                        MovePlate();
                        trigger.Trigger();
                    }
                }
            }
            else
            {
                isTriggered = true;
                MovePlate();
                trigger.Trigger();
            }
        }

        private void GrowingApartLevelLogic(Collider other)
        {
            if (triggeringObject != null) return;

            triggeringObject = other.gameObject;
            if (useRigidbody)
            {
                if (other.TryGetComponent<Rigidbody>(out var touchingObject))
                {
                    if (touchingObject.mass > triggerWeight)
                    {
                        isTriggered = true;
                        MovePlate();
                    }
                }
            }
            else
            {
                isTriggered = true;
                MovePlate();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (triggeringObject != null)
            {
                if (other.gameObject == triggeringObject)
                {
                    targetPos = startPos;
                    triggeringObject = null;
                    isTriggered = false;
                }
            }
        }

        private void MovePlate()
        {
            Vector3 moveVector = Vector3.zero;

            if (movingAxis == 0) moveVector = new Vector3(movingAmount, 0, 0);

            if (movingAxis == 1) moveVector = new Vector3(0, movingAmount, 0);

            if (movingAxis == 2) moveVector = new Vector3(0, 0, movingAmount);

            targetPos = plateMesh.position - moveVector;
        }
    }
}