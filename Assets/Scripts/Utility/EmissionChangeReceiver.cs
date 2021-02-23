using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissionChangeReceiver : MonoBehaviour
{
    Material emissionMaterial;
    // Start is called before the first frame update
    private void Awake() {
        foreach(Material m in GetComponent<MeshRenderer>().materials){
            if(m.name.Contains("Light")){
                emissionMaterial = m;
            }
        }
    }
    public void setEmissionColor(Color newColor){
        emissionMaterial.SetColor("_EmissionColor",newColor);
    }
}
