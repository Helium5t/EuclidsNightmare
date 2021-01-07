﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{

    private CheckPoint[] checkPoints;
    private CheckPoint activeCheckpoint;
    private Transform player;
    // Start is called before the first frame update
    void Start()
    {
        checkPoints = GetComponentsInChildren<CheckPoint>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void activateCheckpoint(CheckPoint c){
        activeCheckpoint = c;
        player.GetComponentInChildren<Respawner>().changeLandingPoint(c.transform.position);
    }

    public void resetPlayerReference(){
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    public void resetPlayerReference(GameObject newPlayer){
        player = newPlayer.transform;
    }
    
}
