using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Trigger))]
public class LaserSensor : MonoBehaviour
    {
    private Trigger trigger;

    [SerializeField] private Color activatedColor;
    [SerializeField] private Color inactiveColor;
    private void Awake() { 
        GetComponent<MeshRenderer>().material = new Material(GetComponent<MeshRenderer>().material);
        GetComponent<MeshRenderer>().material.color=inactiveColor;
        GetComponent<MeshRenderer>().material.SetColor("_EmissionColor",inactiveColor);
        trigger = GetComponent<Trigger>(); }

    public void enter() { 
        GetComponent<MeshRenderer>().material.color =activatedColor;
        GetComponent<MeshRenderer>().material.SetColor("_EmissionColor",activatedColor);
        trigger.enter(); }
    public void leave() { 
        GetComponent<MeshRenderer>().material.color=inactiveColor;
        GetComponent<MeshRenderer>().material.SetColor("_EmissionColor",inactiveColor);
        trigger.leave(); }
    }
