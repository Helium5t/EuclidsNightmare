using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalSensor : MonoBehaviour
{
    [SerializeField] private PortalManager manager;
    public Portal trackedPortal;
    [SerializeField] private List<Portal> managedPortals;
    private List<float> originalSizes;
    [SerializeField][Range(1f,5f)] private float transitionSpeed=2f;
    public bool isFirst = false;
    private bool playerInside = false;

    private void Awake() {
        if(!manager){
            gameObject.SetActive(false);
        }
    }

    private void Start() {
        if(managedPortals.Count >0 && !isFirst){
            originalSizes = new List<float>(managedPortals.Count);
            for(int i =0; i< managedPortals.Count; i++){
                originalSizes.Add(managedPortals[i].transform.localScale.x);
                Vector3 size= managedPortals[i].transform.localScale;
                size.x = 0;
                managedPortals[i].transform.localScale = size;
                size = managedPortals[i].linkedPortal.transform.localScale;
                size.x = 0;
                managedPortals[i].linkedPortal.transform.localScale = size;
                managedPortals[i].gameObject.SetActive(false);
                managedPortals[i].linkedPortal.gameObject.SetActive(false);
            }
        }
        playerInside = isFirst;
        if(isFirst){
            manager.usedPortal(trackedPortal);
        }
    }

    public IEnumerator activatePortal(Portal p, float targetScale){
        p.gameObject.SetActive(true);
        yield return null;
        while(Mathf.Abs(p.transform.localScale.x - targetScale) > 0.001f && playerInside){
            Vector3 scale = p.transform.localScale;
            scale.x = Mathf.Lerp(scale.x,targetScale,Time.deltaTime*transitionSpeed);
            p.transform.localScale = scale;
            yield return null;
        }
        if(playerInside){
            p.transform.localScale = new Vector3(targetScale,p.transform.localScale.y,p.transform.localScale.z);
        }
    }
    public IEnumerator deactivatePortal(Portal p){
        yield return null;
        Debug.Log("Resuming");
        while(p.transform.localScale.x > 0.001f && !playerInside){
            Vector3 scale = p.transform.localScale;
            scale.x = Mathf.Lerp(scale.x,0f,Time.deltaTime*transitionSpeed);
            p.transform.localScale = scale;
            yield return null;
        }
        if(!playerInside){
            p.transform.localScale = new Vector3(0f,p.transform.localScale.y,p.transform.localScale.z);
            p.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")){
            manager.usedPortal(trackedPortal);
            if(!playerInside){
                if(managedPortals.Count>0){
                    foreach(Portal p in managedPortals){
                        float originalSize = originalSizes[managedPortals.IndexOf(p)];
                        StartCoroutine(activatePortal(p,originalSize));
                        StartCoroutine(activatePortal(p.linkedPortal,originalSize));
                    }
                }
                playerInside = true;
            }
        }
    }
    public void playerExited(){
        if(playerInside){
            playerInside = false;
            if(managedPortals.Count>0){
                foreach(Portal p in managedPortals){
                    StartCoroutine("deactivatePortal",p);
                    StartCoroutine("deactivatePortal",p.linkedPortal);
                }
            }
        }
    }
}
