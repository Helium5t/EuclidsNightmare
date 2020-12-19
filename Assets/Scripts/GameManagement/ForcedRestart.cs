using System.Collections;
using System.Collections.Generic;
using GameManagement;
using UnityEngine;

public class ForcedRestart : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")){
            FindObjectOfType<LevelLoader>().GetComponent<LevelLoader>().RestartCurrentLevel();
        }
    }
}
