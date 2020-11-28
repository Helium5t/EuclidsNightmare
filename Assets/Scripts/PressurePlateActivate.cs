using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateActivate : MonoBehaviour
{
 
    [SerializeField]
    GameObject takeUp;
    public bool IsOpened = false;


    void OnTriggerEnter(Collider col)
    {

        if (IsOpened == false)
        {

            takeUp.transform.position += new Vector3(0, -100, 0);

            IsOpened = true;


        }

    }
}


