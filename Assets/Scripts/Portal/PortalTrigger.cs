using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] Portal []portal;
    void OnTriggerEnter(Collider col)
    {
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
            if(Vector3.Distance(tran.position,portal[0].transform.position)< Vector3.Distance(tran.position, portal[1].transform.position))
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
