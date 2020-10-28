using UnityEngine;


// This Script is responsible for handling player movement
// Gravity and speed are both handled here and same goes for jumping
public class PlayerMover : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravityMultiplier = 1.0f;
    [SerializeField] private float gravityForce = 9.8f;
    private float verticalMovement = 0f;
    private CharacterController controller;
    private Vector3 movementDirection;
    private bool canJump = true;

    private static readonly int OpenCloseDoor = Animator.StringToHash("OpenCloseDoor");

    private void Awake()
    {
        // Check if the controller exists, if not create it (avoids dumb bugs and makes life easier for attaching scripts)
        bool controllerCheck = gameObject.TryGetComponent<CharacterController>(out controller);
        if (!controllerCheck)
        {
            gameObject.AddComponent<CharacterController>();
        }

        controller = gameObject.GetComponent<CharacterController>();
    }

    private void Update()
    {
        // ground check for when the player touches the ground, no double jump for now.
        if (controller.isGrounded & !canJump)
        {
            canJump = true;
        }

        float sideMovement = Input.GetAxisRaw("Horizontal") * speed;
        float forwardMovement = Input.GetAxisRaw("Vertical") * speed;
        if (Input.GetButton("Fire1"))
        {
            // If left click is held, walk slower
            sideMovement /= 2.0f;
            forwardMovement /= 2.0f;
        }

        if (Input.GetButton("Fire2"))
        {
            sideMovement *= 2.0f;
            forwardMovement *= 2.0f;
        }

        bool jumpButtonPressed = Input.GetButton("Jump");

        movementDirection = new Vector3(sideMovement, 0, forwardMovement);
        movementDirection = transform.TransformDirection(movementDirection);

        // Jumping
        if (jumpButtonPressed & canJump)
        {
            verticalMovement = jumpForce;
            canJump = false;
        }

        // Gravity
        verticalMovement -= gravityForce * gravityMultiplier * Time.deltaTime;
        // canceling downward force in case the player is already grounded (not needed but good practice)
        if (!(controller.isGrounded & verticalMovement < 0))
        {
            movementDirection.y = verticalMovement;
        }
        else
        {
            movementDirection.y = 0;
        }

        controller.Move(movementDirection * Time.deltaTime);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Door") && Input.GetMouseButtonDown(0))
        {
            Animator doorAnimator = other.GetComponentInParent<Animator>();
            doorAnimator.SetTrigger(OpenCloseDoor);
        }
    }

    public Vector3 GetMovementDirection()
    {
        return Vector3.Normalize(movementDirection);
    }
}