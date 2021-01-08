using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Utility;

namespace GameManagement
{
    public class LevelTrigger : MonoBehaviour
    {
        public enum TriggerMode{Start=-1,End=1,Neither = 0};
        [SerializeField] private LevelLoader levelLoader;
        [SerializeField] public TriggerMode mode = 0;
        

        private void OnValidate() {
            if(!levelLoader){
                Debug.LogError(transform.parent.name+":No levelloader reference, making my own");
                levelLoader = GameObject.FindObjectOfType<LevelLoader>();
            }
        }
        private void Awake() {
            if(!levelLoader){
                Debug.LogError(gameObject.scene.name+"No levelloader reference, making my own");
                levelLoader = GameObject.FindObjectOfType<LevelLoader>();
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if((int)mode == 1){
                    Debug.Log(SceneManager.GetActiveScene().name + " COMPLETED!");
                    PlayEndLevelClip();
                    levelLoader.LoadNextLevel();
                }
                else if((int)mode == -1){
                    if(!levelLoader.currentLevel && gameObject.scene.name != SceneManager.GetActiveScene().name){
                        Debug.LogError("LevelLoader.currentLevel is "+levelLoader.currentLevel);
                        Debug.LogError("Forcing to set "+gameObject.scene.name+" as active");
                        levelLoader.forceStartLevel();
                    }
                    if(GetComponentInParent<Animator>()!= null  && GetComponentInParent<Animator>().GetBool("open")){
                        Debug.Log("Closing Door in "+gameObject.scene.name);
                        GetComponentInParent<Animator>().SetBool("open",false);
                    }
                    else{
                        Debug.Log("not closing door in "+gameObject.scene);
                    }
                    levelLoader.startLevel();
                }
            }
            //else Debug.Log(SceneManager.GetActiveScene().name + " NOT completed");
        }

        private void PlayEndLevelClip()
        {
            FMODUnity.RuntimeManager.PlayOneShot(GameSoundPaths.EndGameSoundPath, gameObject.transform.position);
        }

    }
}