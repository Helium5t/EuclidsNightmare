using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] Portal []portal;

    void OnTriggerEnter(Collider col)
    {
        Debug.Log(transform.position);
        if (col.tag == "Player")
        {
            for (int i=0; i<2; i++)
            {
                portal[i].linkedPortal.gameObject.SetActive(false);
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        Transform tran;
        if (col.tag == "Player")
        {       
            tran = player.transform;
            if(tran.position.x - transform.position.x < 0.0000f)
            {
                portal[0].linkedPortal.gameObject.SetActive(true);
                portal[1].linkedPortal.gameObject.SetActive(false);
            }
            else
            {
                portal[0].linkedPortal.gameObject.SetActive(false);
                portal[1].linkedPortal.gameObject.SetActive(true);
                
            }
        }
    }

}
