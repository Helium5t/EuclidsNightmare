using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceTrigger : MonoBehaviour
{
    [SerializeField] Portal[] portalToDisable;
    [SerializeField] Portal portalToEnable;

    PortalTrigger portalTrigger;

    private void Awake() {
        if(!portalTrigger){
            portalTrigger = GameObject.FindObjectOfType<PortalTrigger>();
        }
    }
    private void Start() {
        portalToDisable[0].gameObject.SetActive(false);
        portalToDisable[1].gameObject.SetActive(false);
        portalToEnable.gameObject.SetActive(false);
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            if(portalTrigger.player != col.gameObject){
                portalTrigger.player = col.gameObject;
            }
            portalToDisable[0].gameObject.SetActive(false);
            portalToDisable[1].gameObject.SetActive(false);
            portalToEnable.gameObject.SetActive(true);
        }
    }

}
