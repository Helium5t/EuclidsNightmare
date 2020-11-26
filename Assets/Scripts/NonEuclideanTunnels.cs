using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonEuclideanTunnels : MonoBehaviour
{

    private float lengthScalingFactor;
    // Start is called before the first frame update
    void Start()
    {
        Transform shortEntry = transform.Find("TunnelShort").Find("PortalShortIn");
        Transform longEntry = transform.Find("TunnelLong").Find("PortalLongIn");
        Transform shortExit = transform.Find("TunnelShort").Find("PortalShortOut");
        Transform longExit = transform.Find("TunnelLong").Find("PortalLongOut");

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
