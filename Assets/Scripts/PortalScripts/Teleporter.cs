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
        Debug.Log(gameObject.name+" "+ gameObject.transform.forward * -1f);

        playerController = movedPlayer.GetComponent<CharacterController>();
        teleportOffset = endPoint.position - gameObject.transform.position;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")){
            Vector3 toPlayer = movedPlayer.position - transform.position;
            if(Vector3.Dot(transform.up, toPlayer) >0f){
                Debug.Log("I am " + gameObject.name + " and i am teleporting");
                playerController.enabled = false;
                movedPlayer.position += teleportOffset;
                playerController.enabled = true;
            }
        }
    }
}
