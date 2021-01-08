using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MainCamera : MonoBehaviour {

    Portal[] portals;
    NonEuclideanTunnel[] nets;

    void Awake () {
        nets = new NonEuclideanTunnel[0];
        portals = new Portal[0];
        StartCoroutine("initializeTrackedElements");
    }

    void OnPreCull () {
        for(int i =0; i<nets.Length;i++){
            try{
            nets[i].updateTrackedCamera();
            }
            catch(MissingReferenceException e){
                Debug.Log("Dead Tunnel at "+i);
            }
        }
        for (int i = 0; i < portals.Length; i++) {
            try{
            portals[i].PrePortalRender ();
            }
            catch(MissingReferenceException e){
                Debug.Log("Dead Portal at " + i);
            }
        }
        for (int i = 0; i < portals.Length; i++) {
            try{
            portals[i].Render ();
            }
            catch(MissingReferenceException e){
                Debug.Log("Dead Portal at " + i);
            }
        }

        for (int i = 0; i < portals.Length; i++) {
            try{
            portals[i].PostPortalRender ();
            }
            catch(MissingReferenceException e){
                Debug.Log("Dead Portal at " + i);
            }
        }

    }

    private IEnumerator initializeTrackedElements(){
        List<Portal> allPortals = PortalUtility.findPortalsInScenes();
        yield return null;
        List<Portal> culledPortals = new List<Portal>();
        foreach(Portal p in allPortals){
            if(p.keepActive || p.gameObject.scene.name == SceneManager.GetActiveScene().name){
                culledPortals.Add(p);
            }
        }
        portals = culledPortals.ToArray();
        yield return null;
        List<NonEuclideanTunnel> allNets =PortalUtility.findNETSInScenes();
        yield return null;
        List<NonEuclideanTunnel> culledNets = new List<NonEuclideanTunnel>();
        foreach(NonEuclideanTunnel n in allNets){
            if(n.gameObject.scene.name == SceneManager.GetActiveScene().name){
                culledNets.Add(n);
            }
        }
        nets = culledNets.ToArray();
    }

    public void resetCamera(){
        StartCoroutine("initializeTrackedElements");
    }


}