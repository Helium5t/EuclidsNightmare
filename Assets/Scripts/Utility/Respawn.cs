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
		}

    private void Start() { cc = GetComponent<CharacterController>(); }

    private CharacterController cc;
    private enum Phase { fade_out, fade_in, none  }
	private Phase respawning = Phase.none;
    private float fade_out  = 0f;
    private float fade_in   = 0f;

    void Update()
        {
        Debug.Log("Respawning: " + respawning);
        Debug.Log("OUT: " + fade_out);
        Debug.Log("IN:  " + fade_in);
        Debug.Log("FOG: " + RenderSettings.fogDensity);
        Debug.Log("___________________");

        switch (respawning)
            {
            case Phase.none:
                if (transform.position.y < deathY)
                    {
                    //TODO lock controls
                    respawning = Phase.fade_out;
                    fade_out = 0;
                    }
                break;

            case Phase.fade_out:
                fade_out += Time.deltaTime;
                if (fade_out < fadeTime) { RenderSettings.fogDensity = Mathf.Pow(fade_out / fadeTime, 3); }
                else
                    {
                    cc.enabled = false;
                    cc.Move(Vector3.zero);
                    transform.position = respawnPoint;
                    cc.enabled = true;
                    respawning = Phase.fade_in; 
                    RenderSettings.fogDensity = 1f; 
                    fade_in = fadeTime;
                    }
                break;

            case Phase.fade_in:
                fade_in -= Time.deltaTime;
                if (fade_in > 0) { RenderSettings.fogDensity = Mathf.Pow(fade_in / fadeTime, 3); }
                else
                    {
                    respawning = Phase.none;
                    RenderSettings.fogDensity = 0f;
                    }
                break;
            }
        }

    }