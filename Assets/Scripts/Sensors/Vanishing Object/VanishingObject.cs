using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider))]
public class VanishingObject : MonoBehaviour
{
    [SerializeField][Range(0,32)] private int vanishLayer = 13;
    public bool vanished = false;
    public bool startVanished = false;
    // Start is called before the first frame update
    
    void Start()
    {   
        if(startVanished && gameObject.layer == 0){
            gameObject.layer = vanishLayer;
            if(transform.childCount>0){
                for(int i =0;i<transform.childCount;i++){
                    transform.GetChild(i).gameObject.layer = vanishLayer;
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
    }

    // Update is called once per frame
    public void resetVanished(){
        if(!startVanished){
            return;
        }
        vanished = true;
        gameObject.layer = vanishLayer;
        if(transform.childCount>0){
            for(int i =0;i<transform.childCount;i++){
                transform.GetChild(i).gameObject.layer = vanishLayer;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player") && gameObject.layer!=0 && vanished){
            vanished = false;
            gameObject.layer = 0;
            if(transform.childCount>0){
                for(int i =0;i<transform.childCount;i++){
                    transform.GetChild(i).gameObject.layer = 0;
                }
            }
        }
    }
}
