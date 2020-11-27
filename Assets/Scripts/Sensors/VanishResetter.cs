using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class VanishResetter : MonoBehaviour
{
    // Start is called before the first frame update
   private void OnTriggerEnter(Collider other) {
       if(other.CompareTag("Player")){
           foreach(VanishingObject v in FindObjectsOfType<VanishingObject>()){
               v.resetVanished();
           }
       }
   }
}
