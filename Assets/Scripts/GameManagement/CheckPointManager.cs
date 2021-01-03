using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{

    private CheckPoint[] checkPoints;
    private CheckPoint activeCheckpoint;
    // Start is called before the first frame update
    void Start()
    {
        checkPoints = GetComponentsInChildren<CheckPoint>();
    }

    public void activateCheckpoint(CheckPoint c){
        activeCheckpoint = c;
        GetComponent<Respawn>().respawnPoint = c.transform.position;
    }
    
}
