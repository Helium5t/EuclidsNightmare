using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

[RequireComponent(typeof(Collider))]
public class VanishingObject : MonoBehaviour
{
    [SerializeField][Range(0,32)] private int vanishLayer = 13;
    private Transform player;
    private VanishGraphics transitionGraphics;
    private SphereCollider rangeDetector;
    private float detectRadius;
    public bool vanished = false;
    public bool startVanished = false;
    [SerializeField][Range(0f,5f)] private float reachedThreshold;
    private bool reachedTop;
    // Start is called before the first frame update

    void Start()
    {   
        vanished = false;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rangeDetector = GetComponentInChildren<SphereCollider>();
        transitionGraphics = GetComponentInChildren<VanishGraphics>();
        if(!transitionGraphics){
            Debug.LogError("NOT FOUND");
            Debug.Break();
        }
        if(startVanished && gameObject.layer == 0){
            gameObject.layer = vanishLayer;
            if(transform.childCount>0){
                for(int i =0;i<transform.childCount;i++){
                    if(transform.GetChild(i).name != "Vanisher"){
                        transform.GetChild(i).gameObject.layer = vanishLayer;
                    }
                }
            }
            vanished = true;
        }
        else{
            if(gameObject.layer == vanishLayer){
                startVanished = true;
                vanished = true;
            }
        }
        transitionGraphics.isVisible = !vanished;
        transitionGraphics.Initialize();
        detectRadius = rangeDetector.radius * transform.localScale.y;
        if(!startVanished) rangeDetector.enabled = false;
    }

    // Update is called once per frame
    public void resetVanished(){
        if(!startVanished || reachedTop){
            return;
        }
        vanished = true;
        gameObject.layer = vanishLayer;
        if(transform.childCount>0){
            for(int i =0;i<transform.childCount;i++){
                if(transform.GetChild(i).name != "Vanisher"){
                        transform.GetChild(i).gameObject.layer = vanishLayer;
                }
            }
        }
        transitionGraphics.isVisible = false;
        transitionGraphics.setVisibility(0f);
    }

    private void OnTriggerExit(Collider other) {
        if(other.CompareTag("Player")){
            transitionGraphics.isVisible = false;
            transitionGraphics.setVisibility(0f);
        }
    }

    private void OnTriggerStay(Collider other) {
        if(other.CompareTag("Player")){
            if(reachedTop){
                transitionGraphics.setVisibility(0f);
                return;
            }
            float distanceFromTop = Vector3.Distance(other.transform.position,rangeDetector.bounds.center);
            if(distanceFromTop < detectRadius){
                if(distanceFromTop > reachedThreshold){
                    float ratio = (distanceFromTop - reachedThreshold) / (detectRadius - reachedThreshold);
                    ratio = 1 - Mathf.Clamp01(ratio);
                    transitionGraphics.setVisibility(ratio);
                }
                else if(!reachedTop){
                        Debug.Log("reached top!");
                        reachedTop = true;
                        transitionGraphics.setVisibility(1);
                        appear();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")){
            transitionGraphics.isVisible = true;
        }

        
    }

    private void appear(){
        vanished = false;
        gameObject.layer = 0;
        if(transform.childCount>0){
            for(int i =0;i<transform.childCount;i++){
                if(transform.GetChild(i).gameObject.layer != 0){
                    transform.GetChild(i).gameObject.layer = 0;
                }
            }
        }
    }
}
