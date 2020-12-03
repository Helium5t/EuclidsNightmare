using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
public class NonEuclideanTunnels : MonoBehaviour
{

    private float lengthScalingFactor;
    // Start is called before the first frame update
    void Start()
    {
        Transform shortEntry,longEntry,shortExit,longExit;
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

        float shortLength = Vector3.Distance(shortEntry.position,shortExit.position);
        float longLength = Vector3.Distance(longEntry.position,longExit.position);
        lengthScalingFactor = longLength / shortLength;
        
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
