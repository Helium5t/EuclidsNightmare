using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Utility;

namespace GameManagement
{
    public class LevelTrigger : MonoBehaviour
    {
        private enum TriggerMode{Start=-1,End=1,Neither = 0};
        [SerializeField] private LevelLoader levelLoader;
        [SerializeField] private TriggerMode mode = 0;
        

        private void Awake() {
            if(!levelLoader){
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
                    if(!levelLoader.currentLevel){
                        Debug.Log("Forcing to set "+gameObject.scene.name+" as active");
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