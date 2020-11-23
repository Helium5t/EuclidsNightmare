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
        ToggleLights(lightsOn ? UnityEngine.Color.black : UnityEngine.Color.yellow);
    }


    private void ToggleLights(Color colorToAssign)
    {
        interrupterMaterial.SetColor(Color, colorToAssign);
        foreach (GameObject lightObject in lights)
        {
            lightObject.SetActive(!lightObject.activeInHierarchy);
        }

        lightsOn = !lightsOn;
    }
}