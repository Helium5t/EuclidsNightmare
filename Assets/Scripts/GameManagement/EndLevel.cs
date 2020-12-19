using GameManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class EndLevel : MonoBehaviour
{   
    [SerializeField] private GameObject levelLoader;

    [Header("Audio Setting")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip endLevelClip;
    [SerializeField] private bool disableAudio;

    private void OnValidate() {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        if(!disableAudio && !endLevelClip){
            disableAudio = true;
            Debug.LogError("No audio clip set for "+ gameObject.name +", disabling audio");
        }
        else{
            audioSource.clip = endLevelClip;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log(SceneManager.GetActiveScene().name + " COMPLETED!");
            PlayEndLevelClip();
            levelLoader.GetComponent<LevelLoader>().LoadNextLevel();
        }
        else Debug.Log(SceneManager.GetActiveScene().name + " NOT completed");
    }
    
    private void PlayEndLevelClip(){
        if(audioSource.mute){
            audioSource.mute = false;
        }
        if(audioSource.loop){
            audioSource.loop = false;
        }
        audioSource.Play();
    }
}