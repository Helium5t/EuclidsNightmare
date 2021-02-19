using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SpeechManager : MonoBehaviour
{
    [SerializeField] private float wordsPerMinute = 250f;
    [SerializeField] [Range(0f, 5f)] private float timeBetweenSpeeches = 1f;

    [SerializeField] private bool beginOnWake = false;
    [SerializeField] private List<string> speeches;
    [SerializeField] private string hintText;
    [SerializeField] private CartManager cart;
    [HideInInspector] public bool speaking = false;
    [HideInInspector] public bool spoken = false;

    private List<Sentence> sentences;
    private bool hinting = false;
    private bool stopSpeech = false;
    private bool stoppedSpeech = false;
    private bool cutSpeech = false;
    private Text speechText;
    private static readonly int fadeTrigger = Animator.StringToHash("FadeTrigger");
    private Animator speechAnimator;
    public bool isPersistent = false;

    // Start is called before the first frame update
    private void Start()
    {
        speechText = GetComponentInChildren<Text>();
        speechAnimator = GetComponentInChildren<Animator>();
        sentences = new List<Sentence>(speeches.Count);
        for(int i =0; i<speeches.Count; i++){
            sentences.Add(new Sentence(speeches[i]));
        }
        if(beginOnWake){
            speaking = true;
            StartCoroutine("speech");
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            StartCoroutine("getHint");
        }
    }

    private IEnumerator getHint()
    {
        if (hintText.Length ==0 || hinting || beginOnWake || gameObject.scene.name != SceneManager.GetActiveScene().name) yield break;
        hinting = true;
        float timeToWait = getUpTimeInSeconds(hintText);
        stopSpeech = true;
        if (speaking)
        {
            Debug.Log("Speaking");
            while (!stoppedSpeech)
            {
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


    private float getUpTimeInSeconds(string readText)
    {
        int words = 0;
        foreach (char c in readText)
        {
            if (c == ' ' || c == '.')
            {
                words++;
            }
        }

        int upTime = Mathf.FloorToInt(words * 60f / wordsPerMinute) + 1;
        return Mathf.Max(upTime, 4f);
    }

    public void TellOneSentence(string theSentence)
    {
         StartCoroutine(OneShotSentence(theSentence));
    }

    private IEnumerator OneShotSentence(string theSentence)
    {
        float timeToWait = getUpTimeInSeconds(theSentence);
        speechText.text = theSentence;
        speechAnimator.SetTrigger(fadeTrigger);
        yield return new WaitForSeconds(timeToWait);
        speechAnimator.SetTrigger(fadeTrigger);
        yield return new WaitForSeconds(timeToWait);
    }


    private IEnumerator speech()
    {
        yield return null;
        while(SceneManager.GetActiveScene().name != gameObject.scene.name){
            yield return new WaitForEndOfFrame();
        }
        for (int i = 0; i < sentences.Count; i++)
        {
            if(cutSpeech){Debug.Log("speech has been cut");break;} 
            string toldSpeech = sentences[i].sentence;
            float timeToWait = getUpTimeInSeconds(toldSpeech);
            while (stopSpeech)
            {
                if (!stoppedSpeech) stoppedSpeech = true;
                yield return new WaitForSeconds(timeToWait);
            }
            stoppedSpeech = false;
            //Debug.Log(timeToWait);
            speechText.text = toldSpeech;
            speechAnimator.SetTrigger(fadeTrigger);
            sentences[i].tell();
            yield return new WaitForSeconds(timeToWait);
            Debug.Log("Fading away");
            speechAnimator.SetTrigger(fadeTrigger);
            yield return new WaitForSeconds(timeBetweenSpeeches);
        }
        speaking = false;
        spoken = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entering "+ gameObject.name + " in " + gameObject.scene.name);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Original number: " + speeches.Count);
            Debug.Log("Current number: " + sentences.Count);
            if (speaking || spoken) return;
            speaking = true;
            StartCoroutine("speech");
            cart?.activateCart();
        }
    }

    public List<Sentence> getUntoldSentences(){
        List<Sentence> untold = new List<Sentence>();
        for(int i=0;i<sentences.Count;i++){
            if(!sentences[i].told){
                untold.Add(sentences[i]);
            }
        }
        return untold;
    }

    public void mergeFromPreviousSpeech(List<Sentence> oldSentences){
        Debug.Log("merging dialogue into "+ gameObject.scene.name);
        List<Sentence> newSentences = new List<Sentence>(oldSentences.Count + sentences.Count);
        newSentences.AddRange(oldSentences);
        newSentences.AddRange(sentences);
        sentences = newSentences;
    }

    public void stopSpeaking(){
        Debug.Log("Cutting speech");
        cutSpeech = true;
    }


}
public class Sentence{
    public string sentence;
    public bool told = false;

    public Sentence(string sent){
        this.sentence = sent;
        told = false;
    }
    public void tell(){
        told = true;
    }
}