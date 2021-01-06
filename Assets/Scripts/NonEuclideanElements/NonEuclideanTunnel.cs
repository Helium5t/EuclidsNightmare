using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Player;
public class NonEuclideanTunnel : MonoBehaviour
{

    private float lengthScalingFactor;
    private Transform shortEntry,longEntry,shortExit,longExit;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        try{
        shortEntry = transform.Find("TunnelShort").Find("PortalShortIn");
        longEntry = transform.Find("TunnelLong").Find("PortalLongIn");
        shortExit = transform.Find("TunnelShort").Find("PortalShortOut");
        longExit = transform.Find("TunnelLong").Find("PortalLongOut");
        }
        catch(NullReferenceException e){
            shortEntry = transform.Find("Ring").Find("RingIn");
            longEntry = transform.Find("RingDestination").Find("DestinationIn");
            shortExit = transform.Find("Ring").Find("RingOut");
            longExit = transform.Find("RingDestination").Find("DestinationOut");
        }
        shortEntry.GetComponentInChildren<Portal>().addAlternateCamera(longExit.GetComponentInChildren<Camera>());
        longEntry.GetComponentInChildren<Portal>().addAlternateCamera(shortExit.GetComponentInChildren<Camera>());
        shortExit.GetComponentInChildren<Portal>().addAlternateCamera(longEntry.GetComponentInChildren<Camera>());
        longExit.GetComponentInChildren<Portal>().addAlternateCamera(shortEntry.GetComponentInChildren<Camera>());
        shortEntry.GetComponentInChildren<Portal>().setActiveCam(-1);
        longEntry.GetComponentInChildren<Portal>().setActiveCam(-1);
        shortExit.GetComponentInChildren<Portal>().setActiveCam(-1);
        longExit.GetComponentInChildren<Portal>().setActiveCam(-1);
        float shortLength = Vector3.Distance(shortEntry.position,shortExit.position);
        float longLength = Vector3.Distance(longEntry.position,longExit.position);
        lengthScalingFactor = longLength / shortLength;
        
    }

    public void updateTrackedCamera(){
        StartCoroutine("updateRoutine");
    }

    private IEnumerator updateRoutine(){
        yield return null;
        if(player.GetComponent<FPSController>().inTunnel){
            shortEntry.GetComponentInChildren<Portal>().setActiveCam(-1);
            longEntry.GetComponentInChildren<Portal>().setActiveCam(-1);
            shortExit.GetComponentInChildren<Portal>().setActiveCam(-1);
            longExit.GetComponentInChildren<Portal>().setActiveCam(-1);
        }
        else{
            float shortEntryDist = Vector3.Distance(shortEntry.position,player.position);
            float longEntryDist = Vector3.Distance(longEntry.position,player.position);
            float minEntryDist = Mathf.Min(shortEntryDist,longEntryDist);
            float shortExitDist = Vector3.Distance(shortExit.position,player.position);
            float longExitDist = Vector3.Distance(longExit.position,player.position);
            float minExitDist = Mathf.Min(shortExitDist,longExitDist);
            if(minEntryDist > minExitDist){
                shortEntry.GetComponentInChildren<Portal>().setActiveCam(0);
                longEntry.GetComponentInChildren<Portal>().setActiveCam(0);
                shortExit.GetComponentInChildren<Portal>().setActiveCam(-1);
                longExit.GetComponentInChildren<Portal>().setActiveCam(-1);
            }
            else{
                shortEntry.GetComponentInChildren<Portal>().setActiveCam(-1);
                longEntry.GetComponentInChildren<Portal>().setActiveCam(-1);
                shortExit.GetComponentInChildren<Portal>().setActiveCam(0);
                longExit.GetComponentInChildren<Portal>().setActiveCam(0);
            }
        }
        yield return null;
    }

    public void reinitPlayer(GameObject newPlayer){
        player = newPlayer.transform;
    }

    public float expandLength(float scaledValue){
        return scaledValue * lengthScalingFactor;
    }

    public Vector3 expandVector(Vector3 scaledVector){
        return scaledVector * lengthScalingFactor;
    }

    public float collapseLength(float scaledValue){
        return scaledValue / lengthScalingFactor;
    }

    public Vector3 collapseVector(Vector3 scaledVector){
        return scaledVector / lengthScalingFactor;
    }

    
}
