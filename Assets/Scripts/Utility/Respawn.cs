using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[ExecuteAlways]
public class Respawn : MonoBehaviour
    {
    [DraggablePoint]
    public Vector3 respawnPoint = new Vector3(0, 100, 0);
    private Vector3 respawnPointOld = new Vector3(0, 100, 0); //used in editor only

    [SerializeField]
    private float deathY = -10;
    [SerializeField]
    private float fadeTime = 3;

    [SerializeField]
    private Color fogColor;

    [SerializeField]
    private Material skyboxFogMaterial;

    private Vector3 landingPoint; //used in editory only

	private void OnDrawGizmosSelected()
        {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(respawnPoint, 1);
        Gizmos.DrawWireSphere(landingPoint, 1);
        Gizmos.DrawLine(respawnPoint, landingPoint);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(respawnPoint.x, deathY, respawnPoint.z), new Vector3(100, 0, 100));
        }

	private void OnValidate()
		{
        if (!respawnPoint.Equals(respawnPointOld))
            {
            respawnPointOld = respawnPoint;
            // update landing point
            RaycastHit hit;
            if (Physics.Raycast(respawnPoint, -Vector3.up, out hit)) { landingPoint = hit.point; }
            else { landingPoint = respawnPoint; }
            }
        RenderSettings.fogColor = fogColor;
		}

    private void Start() { cc = GetComponent<CharacterController>(); setFog(0f); }

    private CharacterController cc;
    private enum Phase { fadeOut, fadeIn, none  }
	private Phase respawning = Phase.none;
    private float fadeOut  = 0f;
    private float fadeIn   = 0f;

    void Update()
        {
        Debug.Log("Respawning: " + respawning);
        Debug.Log("OUT: " + fadeOut);
        Debug.Log("IN:  " + fadeIn);
        Debug.Log("FOG: " + RenderSettings.fogDensity);
        Debug.Log("___________________");

        switch (respawning)
            {
            case Phase.none:
                if (transform.position.y < deathY)
                    {
                    //TODO lock controls
                    respawning = Phase.fadeOut;
                    fadeOut = 0;
                    }
                break;

            case Phase.fadeOut:
                fadeOut += Time.deltaTime;
                if (fadeOut < fadeTime) { setFog(fadeOut / fadeTime); }
                else
                    {
                    cc.enabled = false;
                    cc.Move(Vector3.zero);
                    transform.position = respawnPoint;
                    cc.enabled = true;
                    respawning = Phase.fadeIn;
                    setFog(1f);
                    fadeIn = fadeTime;
                    }
                break;

            case Phase.fadeIn:
                fadeIn -= Time.deltaTime;
                if (fadeIn > 0) { setFog(fadeIn / fadeTime); }
                else
                    {
                    respawning = Phase.none;
                    setFog(0f);
                    }
                break;
            }
        }

    private void setFog(float alpha)
        {
        RenderSettings.fogDensity = Mathf.Pow(alpha, 3f);
        skyboxFogMaterial.color = new Color(skyboxFogMaterial.color.r, skyboxFogMaterial.color.g, skyboxFogMaterial.color.b, alpha);
        }
    }