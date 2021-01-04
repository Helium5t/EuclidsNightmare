using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class PortalUtility {
    public static List<Portal> findPortalsInScenes(){
        return new List<Portal>(Resources.FindObjectsOfTypeAll<Portal>());
        /*List<Portal> foundPortals = new List<Portal>();
        for(int i = 0; i<SceneManager.sceneCount; i++){
            foreach(GameObject g in SceneManager.GetSceneAt(i).GetRootGameObjects()){
                foundPortals.AddRange(g.GetComponentsInChildren<Portal>(true));
            }
        }
        return foundPortals;*/
    }

    public static List<NonEuclideanTunnel> findNETSInScenes(){
        return new List<NonEuclideanTunnel>(Resources.FindObjectsOfTypeAll<NonEuclideanTunnel>());
    }
}
