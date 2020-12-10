using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarTrigger : MonoBehaviour
{
    [SerializeField] GameObject player;
    // Start is called before the first frame update

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            gameObject.layer = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.layer = 0;
            }
           
        }

    }
}
