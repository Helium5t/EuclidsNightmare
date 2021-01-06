using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechManager : MonoBehaviour
{
    [SerializeField] private float wordsPerMinute = 250f;
    [SerializeField][Range(0f,5f)] private float timeBetweenSpeeches = 1f;
    [SerializeField] private List<string> speeches;
    [SerializeField] private string hintText;
    private bool speaking = false;
    private bool spoken = false;
    private bool hinting = false;
    private bool stopSpeech = false;
    private bool stoppedSpeech = false;
    private Text speechText;
    private static readonly int fadeTrigger = Animator.StringToHash("FadeTrigger");
    private Animator speechAnimator;
    // Start is called before the first frame update
    void Start()
    {
        speechText = GetComponentInChildren<Text>();
        speechAnimator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.H)){
            StartCoroutine("getHint");
        }
    }
    
    private IEnumerator getHint(){
        if(hinting) yield break;
        hinting = true;
        float timeToWait = getUpTimeInSeconds(hintText);
        stopSpeech = true;
        if(speaking){
            Debug.Log("Speaking");
            while(!stoppedSpeech){
                yield return new WaitForSeconds(1f);
            }
        }
        speechText.text = hintText;
        speechAnimator.SetTrigger(fadeTrigger);
        yield return new WaitForSeconds(timeToWait);
        speechAnimator.SetTrigger(fadeTrigger);
        yield return new WaitForSeconds(timeBetweenSpeeches);
        stopSpeech = false;
        hinting = false;
    }
    

    private float getUpTimeInSeconds(string readText){
        int words = 0;
        foreach(char c in readText){
            if(c == ' ' || c== '.'){
                words++;
            }
        }
        int upTime = Mathf.FloorToInt(words*60f/wordsPerMinute) +1;
        return Mathf.Max(upTime,4f);
    }



    private IEnumerator speech(){
        yield return null;
        for(int i = 0; i<speeches.Count;i++){
            float timeToWait = getUpTimeInSeconds(speeches[i]);
            while(stopSpeech){
                if(!stoppedSpeech) stoppedSpeech = true;
                yield return new WaitForSeconds(timeToWait);
            }
            stoppedSpeech = false;
            Debug.Log(timeToWait);
            speechText.text = speeches[i];
            speechAnimator.SetTrigger(fadeTrigger);
            yield return new WaitForSeconds(timeToWait);
            Debug.Log("Fading away");
            speechAnimator.SetTrigger(fadeTrigger);
            yield return new WaitForSeconds(timeBetweenSpeeches);
        }
        speaking = false;
        spoken = true;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")){
            if(speaking || spoken) return;
            speaking = true;
            StartCoroutine("speech");
        }
    }
}
