using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Trigger))]
public class ExamplePressurePlate : MonoBehaviour
    {
    private Trigger trigger;
    private void Awake() { trigger = GetComponent<Trigger>(); }

    [SerializeField]
    KeyCode key;

	void Update()
        {
        //on enter collision
        if (Input.GetKeyDown(key)) { trigger.enter(); }

        //on leave collision
        if (Input.GetKeyUp(key)) { trigger.leave(); }
        }
    }