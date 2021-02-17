using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using GameManagement;

public class FallingCam : MonoBehaviour
{
    
    public bool fall = false;

    private CinemachineBrain cinemachineBrain;

    [SerializeField] private Vector3 impactPoint;

    [SerializeField] private SpeechManager speechManager;
    [SerializeField] private LevelLoader loader;
    private float speed = 0f;

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position + impactPoint,Vector3.one * 2f);
    }

    private void Start() {
        cinemachineBrain = GetComponent<CinemachineBrain>();
        StartCoroutine("observeDialogue");
        impactPoint = transform.position + impactPoint;
    }
    private void Update() {
        if(!fall) return;
        speed += 9.8f * Time.deltaTime;
        Vector3 nextPos = transform.position - Vector3.up * speed;
        nextPos.y = Mathf.Max(nextPos.y,impactPoint.y);
        transform.position = nextPos;
        if(transform.position.y<= impactPoint.y){
            fall = false;
            loader.LoadNextLevel();
        }
    }

    private IEnumerator observeDialogue(){
        yield return null;
        while(!speechManager.spoken){
            yield return new WaitForEndOfFrame();
        }
        fall = true;
        Debug.Log("Start to fall");
        cinemachineBrain.enabled = false;
        yield return null;
    }
}
