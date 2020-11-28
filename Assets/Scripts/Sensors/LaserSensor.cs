using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Trigger))]
public class LaserSensor : MonoBehaviour
    {
    private Trigger trigger;
    private void Awake() { trigger = GetComponent<Trigger>(); }

    public void enter() { trigger.enter(); }
    public void leave() { trigger.leave(); }
    }
