using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MainCamera : MonoBehaviour {

    Portal[] portals;
    NonEuclideanTunnel[] nets;

    void Awake () {
        initializeTrackedElements();
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

    private void initializeTrackedElements(){
        List<Portal> allPortals = PortalUtility.findPortalsInScenes();
        List<Portal> culledPortals = new List<Portal>();
        foreach(Portal p in allPortals){
            if(p.keepActive || p.gameObject.scene.name == SceneManager.GetActiveScene().name){
                culledPortals.Add(p);
            }
        }
        portals = culledPortals.ToArray();
        nets = FindObjectsOfType<NonEuclideanTunnel>();
    }

    public void resetCamera(){
        initializeTrackedElements();
    }


}