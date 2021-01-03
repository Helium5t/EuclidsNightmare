using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using Player;

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
	private float waitTime = 1;

	[SerializeField]
	private Color fogColor;

	[SerializeField]
	private Material skyboxFogMaterial;

	[SerializeField]
	private MeshFilter skyboxCoverMeshFilter;

	private Vector3 landingPoint; //used in editory only

    [SerializeField] private bool activeLogging = false;

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

	private void Start()
		{
		cc = GetComponent<CharacterController>(); setFog(0f);
		fpsc = GetComponent<Player.FPSController>(); setFog(0f);

		Mesh skyboxCoverMesh = new Mesh();
		skyboxCoverMesh.vertices = skyboxCoverMeshFilter.mesh.vertices;
		skyboxCoverMesh.triangles = skyboxCoverMeshFilter.mesh.triangles;
		skyboxCoverMesh.uv = skyboxCoverMeshFilter.mesh.uv;
		skyboxCoverMesh.normals = skyboxCoverMeshFilter.mesh.normals.Reverse().ToArray();
		skyboxCoverMesh.colors = skyboxCoverMeshFilter.mesh.colors;
		skyboxCoverMesh.tangents = skyboxCoverMeshFilter.mesh.tangents;
		skyboxCoverMeshFilter.mesh = skyboxCoverMesh;
		}

	private Player.FPSController fpsc;
	private CharacterController cc;
	private enum Phase { fadeOut, wait, fadeIn, landing, none }
	private Phase respawning = Phase.none;
	private float fadeOut = 0f;
	private float wait = 0f;
	private float fadeIn = 0f;

	void Update()
		{
		switch (respawning)
			{
			case Phase.none:
				if (transform.position.y < deathY) { respawning = Phase.fadeOut; fadeOut = 0f; }
				break;

			case Phase.fadeOut:
				fadeOut += Time.deltaTime;
				if (fadeOut < fadeTime) { setFog(fadeOut / fadeTime); }
				else
					{
					tpToRespawnPoint();

					respawning = Phase.wait;
					setFog(1f);
					wait = waitTime;
                    fpsc.lockUntilGround = true;
					}
				break;

			case Phase.wait:
				wait -= Time.deltaTime;
				if (wait <= 0f)
					{
					respawning = Phase.fadeIn;
					setFog(1f);
					fadeIn = fadeTime;
					}
				break;

			case Phase.fadeIn:
				fadeIn -= Time.deltaTime;
				if (fadeIn > 0f) { setFog(fadeIn / fadeTime); }
				else { respawning = Phase.landing; setFog(0f); }
				break;

			case Phase.landing:
				if (cc.isGrounded) { respawning = Phase.none; }
				break;
			}
		}

	private void setFog(float alpha)
		{
		RenderSettings.fogDensity = Mathf.Pow(alpha, 2f);
		skyboxFogMaterial.color = new Color(skyboxFogMaterial.color.r, skyboxFogMaterial.color.g, skyboxFogMaterial.color.b, 1f - Mathf.Pow(alpha - 1f, 6f));
		}

	private void tpToRespawnPoint()
		{
		transform.position = respawnPoint;
		}
	}
