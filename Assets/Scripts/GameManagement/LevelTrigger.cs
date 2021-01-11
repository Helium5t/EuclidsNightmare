using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Utility;
using UnityEditor;

namespace GameManagement
{
    public class LevelTrigger : MonoBehaviour
    {
        public enum TriggerMode{Start=-1,End=1,Neither = 0};
        [SerializeField] private LevelLoader levelLoader;
        [SerializeField] public TriggerMode mode = 0;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if((int)mode == 1){
                    Debug.Log(SceneManager.GetActiveScene().name + " COMPLETED!");
                    PlayEndLevelClip();
                    if(gameObject.scene.buildIndex == SceneManager.GetActiveScene().buildIndex)levelLoader.LoadNextLevel();
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

#region Debug things, to be deleted
        /*
        private void OnValidate() {
            if(levelLoader==null){
                Debug.LogError(transform.name+":No levelloader reference on validate, making my own "+transform.position );
                if(transform.parent) Debug.LogError(transform.parent.name+" in " + gameObject.scene.name + ":"+transform.parent.position);
                levelLoader = GameObject.FindObjectOfType<LevelLoader>();
            }
        }
        private void Start() {
             if(levelLoader==null){
                Debug.LogError(transform.name+":No levelloader reference on validate, making my own "+transform.position );
                if(transform.parent) Debug.LogError(transform.parent.name+" in " + gameObject.scene.name + ":"+transform.parent.position);
                levelLoader = findLevelLoader();
            }
        }
        
        private LevelLoader findLevelLoader(){
            foreach(GameObject g in gameObject.scene.GetRootGameObjects()){
                if(g.GetComponent<LevelLoader>()) return g.GetComponent<LevelLoader>();
            }
            Debug.LogError("LevelLoader not found in " +gameObject.scene.name);
            return null;
        }*/
#endregion
    }
}