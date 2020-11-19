using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class corridor : MonoBehaviour
{
    [SerializeField]
    GameObject takeUp;
    bool IsOpened = false;


    void OnTriggerEnter(Collider col)
    {

        if (IsOpened == false)
        {

            takeUp.transform.position += new Vector3(0, -100, 0);

            IsOpened = true;
           

        }
        
    }
}
