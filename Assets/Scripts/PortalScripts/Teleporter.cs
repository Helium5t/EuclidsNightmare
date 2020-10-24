using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] private Transform endPoint;
    [SerializeField] private Transform movedPlayer;
    private CharacterController playerController;
    private Vector3 teleportOffset;

    private void Start() {
        playerController = movedPlayer.GetComponent<CharacterController>();
        teleportOffset = endPoint.position - gameObject.transform.position;
    }


    private void OnTriggerEnter(Collider other) {
        //Debug.Log(other.name + " started touching " + gameObject.name.Substring(13) + " of " + transform.parent.parent.name);
    }
    private void OnTriggerExit(Collider other) {
        //Debug.Log(other.name + " stopped touching " + gameObject.name.Substring(13) + " of " + transform.parent.parent.name);
        if(other.CompareTag("Player")){
            Vector3 toPlayer = movedPlayer.position - transform.position;
            Vector3 playerDirection = movedPlayer.gameObject.GetComponent<PlayerMover>().GetMovementDirection();
            if(Vector3.Dot(transform.up, playerDirection) < 0f){
                //Debug.Log("Triggered " + gameObject.name.Substring(13) + " of " +transform.parent.parent.name.Substring(6));
                playerController.enabled = false;
                //movedPlayer.position += teleportOffset;
                other.transform.position = other.transform.position + teleportOffset;
                playerController.enabled = true;
            }
            else{
                Debug.Log(gameObject.name.Substring(13) + " of " + transform.parent.parent.name.Substring(6) +" :Wrong Direction");
            }
        }
    }
}
