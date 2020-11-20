using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceTrigger : MonoBehaviour
{
    [SerializeField] Portal []portalToDisable;
    [SerializeField] Portal portalToEnable;
    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            portalToDisable[0].gameObject.SetActive(false);
            portalToDisable[1].gameObject.SetActive(false);
            portalToEnable.gameObject.SetActive(true);
        }
    }

}
