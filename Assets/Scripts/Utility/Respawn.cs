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

    [Header("Prefab only fields - do not change outside of prefab editor")]
    [SerializeField]
    private Image image;

    private Vector3 landingPoint;

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

    CharacterController cc;
	private void Start()
		{
        image.canvasRenderer.SetAlpha(0);
        cc = GetComponent<CharacterController>();
        }

	private bool respawning = false;

    void Update()
        {
        if (!respawning && transform.position.y < deathY)
            {
            respawning = true;
            image.CrossFadeAlpha(1, fadeTime, true);
            StartCoroutine(respawn());
            }
        }

    private IEnumerator respawn()
        {
        yield return new WaitForSeconds(fadeTime);
        transform.position = respawnPoint;
        StartCoroutine(wait());
        }

    private IEnumerator wait()
        {
        image.CrossFadeAlpha(0, fadeTime, true);
        yield return new WaitForSeconds(fadeTime);
        StartCoroutine(finish());
        }
    private IEnumerator finish()
        {
        yield return new WaitForSeconds(fadeTime / 2);
        respawning = false;
        }
    }