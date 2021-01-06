using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    [SerializeField] private List<PortalSensor> portalSensors;
    [SerializeField] private List<Portal> portals;

    public Portal lastUsedPortal{get;private set;}
    private List<float> originalPortalSize;
    private bool playerInside = false;

    [SerializeField][Range(1f,5f)] private float transitionSpeed = 2f;
    // Start is called before the first frame update

    private void Awake() {
        originalPortalSize = new List<float>();
        for(int i=0; i<portals.Count; i++){
            originalPortalSize.Add(portals[i].transform.localScale.x);
        }
    }

    private void Start() {
        Portal firstPortal = null;
        foreach(PortalSensor ps in portalSensors){
            if(ps.isFirst){
                firstPortal = ps.trackedPortal;
                break;
            }
        }
        foreach(Portal p in portals){
            if(p != firstPortal){
                p.transform.localScale = new Vector3(0f,p.transform.localScale.y,p.transform.localScale.z);
                p.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")){
            playerInside = true;
            foreach(PortalSensor ps in portalSensors){
                ps.playerExited();
            }
            foreach(Portal p in portals){
                if(p!=lastUsedPortal){
                    p.gameObject.SetActive(true);
                    float originalScale = originalPortalSize[portals.IndexOf(p)];
                    StartCoroutine(activatePortal(p,originalScale));
                }
            }
        }
    }

    public void exited(Portal usedPortal) {
        Debug.Log("Exiting");
        lastUsedPortal = usedPortal;
        foreach(Portal p in portals){
            if(p!=lastUsedPortal){
                StartCoroutine("deactivatePortal",p);
            }
        }
        playerInside = false;
        Debug.Log("Player Out");
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
        if(playerInside) p.transform.localScale = new Vector3(targetScale,p.transform.localScale.y,p.transform.localScale.z);
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

    public void usedPortal(Portal p){
        Portal usedPortal;
        if(portals.Contains(p)){
            usedPortal = p;
        }
        else if(portals.Contains(p.linkedPortal)){
            usedPortal = p.linkedPortal;
        }
        else{
            Debug.LogError("No portal found");
            return;
        }
        if(playerInside) exited(usedPortal);

    }

}
