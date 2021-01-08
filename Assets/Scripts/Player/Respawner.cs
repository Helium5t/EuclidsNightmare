using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Player;

public class Respawner : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float respawnHeightGain = 100f;
    [SerializeField] private float transitionTime = 1f;
    [SerializeField] private Transform coverImage;
    public float deathHeight = -1000f;
    private Transform landingObject;
    private Vector3 respawnPoint;
    private Animator fadeAnimator;
    private static readonly int fadeTrigger = Animator.StringToHash("FadeTrigger");
    private Vector3 landingPoint;
    private FPSController player;

    private float cubeSize = 0.5f;

    private bool respawning = false;

    // Debug
    private bool playing = false;

#region Editor Mode Functions
    private void OnDrawGizmos() {
        if(playing) return;
        Gizmos.color = Color.green;
        Gizmos.DrawCube(landingObject.position,Vector3.one *cubeSize);
        recomputeRespawnPoint();
        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(respawnPoint,Vector3.one *cubeSize);
        if(landingPoint!=landingObject.position){
            landingPoint = landingObject.position;
        }
    }
    private void OnDrawGizmosSelected() {
        if(playing) return;
        Color gColor = Color.blue;
        gColor.a = 0.6f;
        Gizmos.color = gColor;
        Vector3 deathPoint = new Vector3(transform.position.x,deathHeight,transform.position.z);
        Gizmos.DrawWireCube(deathPoint,new Vector3(100,0,100));
    }

    private void OnValidate() {
        if(playing) return;
        landingObject = transform.Find("landingObject");
        if(!landingObject){
            GameObject respawnCube = new GameObject("landingObject");
            respawnCube.transform.parent = transform;
            landingObject = respawnCube.transform;
            landingObject.localPosition = Vector3.zero;
            landingObject.localScale = Vector3.one;
            landingObject.localRotation = Quaternion.Euler(0f,0f,0f);
        }
        recomputeRespawnPoint();
        
    }
#endregion
    

    private void Awake() {
        playing = true;
        if(landingObject){
            landingPoint = landingObject.position;
        }
        else{
            landingPoint = transform.position;
        }
        recomputeRespawnPoint();
        player = GetComponentInParent<FPSController>();
        if(!coverImage){
            fadeAnimator = transform.Find("RespawnCover").GetComponent<Animator>();
        }
        else{
            fadeAnimator = coverImage.GetComponent<Animator>();
        }
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
        Debug.Log(respawnPoint);
        player.Respawn(respawnPoint);
        fadeAnimator.SetTrigger(fadeTrigger);
        yield return new WaitForSeconds(transitionTime);
        respawning = false;
    }


    public void changeLandingPoint(Vector3 newPos){
        landingPoint = newPos;
        recomputeRespawnPoint();
    }

    private void recomputeRespawnPoint(){
        Physics.Raycast(landingPoint,Vector3.up,out RaycastHit respHit,respawnHeightGain);
        if(respHit.point!=Vector3.zero){
            respawnPoint = respHit.point;
        }
        else{
            respawnPoint = landingPoint + Vector3.up*respawnHeightGain;
        }
    }
}
