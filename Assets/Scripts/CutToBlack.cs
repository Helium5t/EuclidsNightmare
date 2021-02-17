using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameManagement;
public class CutToBlack : Target
{   
    [SerializeField] private LevelLoader loader;
    private void Awake() {
        for(int i=0; i<transform.childCount;i++){
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    protected override void activate()
    {
        Debug.Log("Launched");
        for(int i=0; i<transform.childCount;i++){
            transform.GetChild(i).gameObject.SetActive(true);
        }
        loader.LoadNextLevel();

    }

    protected override void deactivate()
    {
        return;
    }

}
