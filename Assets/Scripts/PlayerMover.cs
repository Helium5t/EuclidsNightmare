using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This Script is responsible for handling player movement
// Gravity and speed are both handled here and same goes for jumping
public class PlayerMover : MonoBehaviour
{
    
    private Vector2 position;
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravityMultiplier = 1.0f;
    private float verticalMovement = 0f;
    CharacterController controller;
    private bool canJump = true;
    void Start()
    {
        // Check if the controller exists, if not create it (avoids dumb bugs and makes life easier for attaching scripts)
        bool controllerCheck = gameObject.TryGetComponent<CharacterController>(out controller);
        if(!controllerCheck){
            gameObject.AddComponent<CharacterController>();
        }
        controller = gameObject.GetComponent<CharacterController>();
    }

    void Update()
    {

        // ground check for when the player touches the ground
        if(!canJump & controller.isGrounded){
            canJump = true;
        }
        float sideMovement = Input.GetAxisRaw("Horizontal")*speed;
        float forwardMovement = Input.GetAxisRaw("Vertical")*speed;
        bool jump = Input.GetButton("Jump");
        Vector3 movementDirection = new Vector3(sideMovement,0,forwardMovement);
        movementDirection = transform.TransformDirection(movementDirection);
        // jumping
        if(jump & canJump){
          verticalMovement = jumpForce;
            canJump = false;
        }
        // gravity
        verticalMovement -= 9.8f * gravityMultiplier * Time.deltaTime;
        // canceling downward force in case the player is already grounded (not needed but good practice)
        if(!(controller.isGrounded & verticalMovement<0)){
            movementDirection.y = verticalMovement;
        }
        else{
            movementDirection.y = 0;
        }
        controller.Move(movementDirection*Time.deltaTime);
        
        
    }

}
