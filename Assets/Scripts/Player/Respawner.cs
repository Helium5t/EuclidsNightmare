using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class Respawner : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float respawnHeight = 100f;
    [SerializeField] private float transitionTime = 1f;
    private Transform respawnPoint;
    private Animator fadeAnimator;
    private static readonly int fadeTrigger = Animator.StringToHash("FadeTrigger");
    private Vector3 landingPoint;
    public float deathHeight;
    private FPSController player;

    private bool respawning = false;
    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(respawnPoint.position,Vector3.one);
        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(landingPoint,Vector3.one);
        recomputeLandingPoint();
    }
    private void OnDrawGizmosSelected() {
        Color gColor = Color.blue;
        gColor.a = 0.6f;
        Gizmos.color = gColor;
        Vector3 deathPoint = new Vector3(transform.position.x,deathHeight,transform.position.z);
        Gizmos.DrawWireCube(deathPoint,new Vector3(100,0,100));
    }

    private void OnValidate() {
        if(!respawnPoint){
            respawnPoint = transform.Find("respawnPoint");
        }
        if(!respawnPoint){
            GameObject respawnCube = new GameObject("respawnPoint");
            respawnCube.transform.parent = transform;
            respawnPoint = respawnCube.transform;
            respawnPoint.localPosition = Vector3.zero;
            respawnPoint.localScale = Vector3.one;
            respawnPoint.localRotation = Quaternion.Euler(0f,0f,0f);
        }
        recomputeLandingPoint();
    }

    private void recomputeLandingPoint(){
        Physics.Raycast(respawnPoint.position,Vector3.down,out RaycastHit landHit,500f);
        landingPoint = landHit.point;
    }

    private void Awake() {
        fadeAnimator = GetComponentInChildren<Animator>();
        player = GetComponentInParent<FPSController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(player.transform.position.y < deathHeight && !respawning){
            respawning = true;
            StartCoroutine("respawnGraphicRoutine");
        }
    }

    private IEnumerator respawnGraphicRoutine(){
        fadeAnimator.SetTrigger(fadeTrigger);
        yield return new WaitForSeconds(transitionTime);
        player.Respawn(respawnPoint.position);
        fadeAnimator.SetTrigger(fadeTrigger);
        yield return new WaitForSeconds(transitionTime);
        respawning = false;
    }


    public void changeLandingPoint(Vector3 newPos){
        landingPoint = newPos;
        respawnPoint.position = landingPoint + Vector3.up*respawnHeight;
    }
}
