using UnityEngine;

public class MainCamera : MonoBehaviour {

    Portal[] portals;
    NonEuclideanTunnel[] nets;

    void Awake () {
        portals = FindObjectsOfType<Portal> ();
        nets = FindObjectsOfType<NonEuclideanTunnel>();
    }

    void OnPreCull () {
        for(int i =0; i<nets.Length;i++){
            nets[i].updateTrackedCamera();
        }
        for (int i = 0; i < portals.Length; i++) {
            portals[i].PrePortalRender ();
        }
        for (int i = 0; i < portals.Length; i++) {
            portals[i].Render ();
        }

        for (int i = 0; i < portals.Length; i++) {
            portals[i].PostPortalRender ();
        }

    }

}