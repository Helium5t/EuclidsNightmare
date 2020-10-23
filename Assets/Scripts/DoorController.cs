using UnityEngine;

public class DoorController : MonoBehaviour
{
    private Animator doorAnimator;
    
    private static readonly int OpenCloseDoor = Animator.StringToHash("OpenCloseDoor");

    private void Awake()
    {
        doorAnimator = GetComponent<Animator>();
    }

    private void OnMouseDown()
    {
        
        doorAnimator.SetTrigger(OpenCloseDoor);
        
    }
}