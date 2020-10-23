using System;
using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    [SerializeField] private GameObject[] lights;

    private bool lightsOn = true;
    private Material interrupterMaterial;

    private static readonly int Color = Shader.PropertyToID("_Color");

    private void Awake()
    {
        interrupterMaterial = gameObject.GetComponent<Renderer>().material;
        interrupterMaterial.SetColor(Color, UnityEngine.Color.yellow);
    }

    private void OnMouseDown()
    {
        if (lightsOn)
        {
            interrupterMaterial.SetColor(Color, UnityEngine.Color.black);
            foreach (GameObject lightObject in lights)
            {
                lightObject.SetActive(false);
            }

            lightsOn = false;
        }
        else
        {
            interrupterMaterial.SetColor(Color, UnityEngine.Color.yellow);
            foreach (GameObject lightObject in lights)
            {
                lightObject.SetActive(true);
            }

            lightsOn = true;
        }
    }
}